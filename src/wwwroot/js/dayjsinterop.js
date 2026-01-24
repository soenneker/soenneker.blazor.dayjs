export class DayJsInterop {
    constructor() {
        this.subscribers = new Map();
        this.timerId = null;
        this.baseIntervalMs = 1000;
        this.nextId = 1;
        this.initialized = false;
        this.tick = this.tick.bind(this);
        this.onVisibilityChange = this.onVisibilityChange.bind(this);

        if (typeof document !== "undefined") {
            document.addEventListener("visibilitychange", this.onVisibilityChange);
        }
    }

    ensureDayJs() {
        const dayjs = window.dayjs;

        if (!dayjs) {
            throw new Error("Day.js not found. Add dayjs + plugins to the page before using Soenneker.Blazor.Dayjs.");
        }

        if (!this.initialized) {
            if (window.dayjs_plugin_utc) {
                dayjs.extend(window.dayjs_plugin_utc);
            }

            if (window.dayjs_plugin_timezone) {
                dayjs.extend(window.dayjs_plugin_timezone);
            }

            if (window.dayjs_plugin_relativeTime) {
                dayjs.extend(window.dayjs_plugin_relativeTime);
            }

            if (window.dayjs_plugin_duration) {
                dayjs.extend(window.dayjs_plugin_duration);
            }

            this.initialized = true;
        }

        return dayjs;
    }

    withTimezone(dayjsInstance, timezone) {
        const dayjs = this.ensureDayJs();

        if (timezone && dayjs.tz) {
            return dayjsInstance.tz(timezone);
        }

        return dayjsInstance;
    }

    getNow(timezone) {
        const dayjs = this.ensureDayJs();

        if (timezone && dayjs.tz) {
            return dayjs().tz(timezone);
        }

        return dayjs();
    }

    normalizeInterval(intervalMs) {
        const parsed = Number(intervalMs);

        if (!Number.isFinite(parsed) || parsed <= 0) {
            return 1000;
        }

        return Math.max(50, parsed);
    }

    recomputeBaseInterval() {
        let min = 1000;

        for (const sub of this.subscribers.values()) {
            if (sub.intervalMs < min) {
                min = sub.intervalMs;
            }
        }

        min = Math.max(50, min);

        if (min !== this.baseIntervalMs) {
            this.baseIntervalMs = min;
            this.restartTimer();
        }
    }

    restartTimer() {
        if (this.timerId) {
            clearTimeout(this.timerId);
            this.timerId = null;
        }

        if (this.subscribers.size > 0) {
            this.scheduleNextTick();
        }
    }

    startTimer() {
        if (this.timerId) {
            return;
        }

        this.scheduleNextTick();
    }

    stopTimerIfIdle() {
        if (this.subscribers.size === 0 && this.timerId) {
            clearTimeout(this.timerId);
            this.timerId = null;
        }
    }

    safeUpdate(sub, value) {
        try {
            sub.dotNetRef.invokeMethodAsync("OnUpdate", value);
        } catch (error) {
            console.error("Dayjs subscription update failed", error);
        }
    }

    computeValue(sub) {
        const dayjs = this.ensureDayJs();

        switch (sub.type) {
            case "now": {
                const now = this.getNow(sub.timezone);
                return now.format(sub.format);
            }
            case "relative": {
                const target = this.withTimezone(dayjs(sub.value), sub.timezone);
                const base = this.getNow(sub.timezone);
                return target.from(base, sub.withoutSuffix);
            }
            case "until": {
                const target = this.withTimezone(dayjs(sub.value), sub.timezone);
                const base = this.getNow(sub.timezone);
                let diff = target.diff(base);

                if (sub.clampToZero && diff < 0) {
                    diff = 0;
                }

                if (dayjs.utc) {
                    return dayjs.utc(diff).format(sub.format);
                }

                return dayjs(diff).format(sub.format);
            }
            default:
                return "";
        }
    }

    publish(sub) {
        const value = this.computeValue(sub);
        this.safeUpdate(sub, value);
    }

    scheduleNextTick() {
        if (this.subscribers.size === 0) {
            return;
        }

        const now = (typeof performance !== "undefined" && performance.now) ? performance.now() : Date.now();
        const base = this.baseIntervalMs;
        const delay = base - (now % base);

        this.timerId = setTimeout(this.tick, delay);
    }

    tick() {
        const now = Date.now();

        for (const sub of this.subscribers.values()) {
            const due = Math.floor(now / sub.intervalMs) * sub.intervalMs;
            if (due > sub.lastTick) {
                sub.lastTick = due;
                this.publish(sub);
            }
        }

        this.timerId = null;
        this.scheduleNextTick();
    }

    onVisibilityChange() {
        if (document.visibilityState !== "visible") {
            return;
        }

        const now = Date.now();
        for (const sub of this.subscribers.values()) {
            sub.lastTick = now;
            this.publish(sub);
        }
    }

    createSubscription(subscriber) {
        const id = this.nextId++;
        const intervalMs = this.normalizeInterval(subscriber.intervalMs);

        const sub = {
            id,
            type: subscriber.type,
            format: subscriber.format,
            timezone: subscriber.timezone,
            value: subscriber.value,
            withoutSuffix: subscriber.withoutSuffix === true,
            clampToZero: subscriber.clampToZero !== false,
            intervalMs,
            lastTick: 0,
            dotNetRef: subscriber.dotNetRef
        };

        this.subscribers.set(id, sub);
        this.recomputeBaseInterval();
        this.startTimer();
        this.publish(sub);

        return id;
    }

    init() {
        this.ensureDayJs();
    }

    format(value, format, timezone) {
        const dayjs = this.ensureDayJs();
        return this.withTimezone(dayjs(value), timezone).format(format);
    }

    fromNow(value, withoutSuffix, timezone) {
        const dayjs = this.ensureDayJs();
        const target = this.withTimezone(dayjs(value), timezone);
        const base = this.getNow(timezone);
        return target.from(base, withoutSuffix === true);
    }

    toNow(value, withoutSuffix, timezone) {
        const dayjs = this.ensureDayJs();
        const target = this.withTimezone(dayjs(value), timezone);
        const base = this.getNow(timezone);
        return target.to(base, withoutSuffix === true);
    }

    add(value, amountMs, format, timezone) {
        const dayjs = this.ensureDayJs();
        const added = dayjs(value).add(amountMs, "ms");
        return this.withTimezone(added, timezone).format(format);
    }

    subtract(value, amountMs, format, timezone) {
        const dayjs = this.ensureDayJs();
        const subtracted = dayjs(value).subtract(amountMs, "ms");
        return this.withTimezone(subtracted, timezone).format(format);
    }

    durationHumanize(amountMs, withoutSuffix) {
        const dayjs = this.ensureDayJs();
        return dayjs.duration(amountMs).humanize(withoutSuffix === true);
    }

    until(value, format, timezone, clampToZero) {
        const dayjs = this.ensureDayJs();
        const target = this.withTimezone(dayjs(value), timezone);
        const base = this.getNow(timezone);
        let diff = target.diff(base);

        if (clampToZero === true && diff < 0) {
            diff = 0;
        }

        if (dayjs.utc) {
            return dayjs.utc(diff).format(format);
        }

        return dayjs(diff).format(format);
    }

    subscribeNow(format, timezone, intervalMs, dotNetRef) {
        return this.createSubscription({
            type: "now",
            format,
            timezone,
            intervalMs,
            dotNetRef
        });
    }

    subscribeRelative(value, intervalMs, withoutSuffix, timezone, dotNetRef) {
        return this.createSubscription({
            type: "relative",
            value,
            intervalMs,
            withoutSuffix,
            timezone,
            dotNetRef
        });
    }

    subscribeUntil(value, format, intervalMs, timezone, clampToZero, dotNetRef) {
        return this.createSubscription({
            type: "until",
            value,
            format,
            intervalMs,
            timezone,
            clampToZero,
            dotNetRef
        });
    }

    unsubscribe(id) {
        this.subscribers.delete(id);
        this.recomputeBaseInterval();
        this.stopTimerIfIdle();
    }
}

window.DayJsInterop = new DayJsInterop();
