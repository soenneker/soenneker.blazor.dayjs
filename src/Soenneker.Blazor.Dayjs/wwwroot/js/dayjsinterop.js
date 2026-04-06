const interop = (() => {
    const instance = {};
    instance.ensureDayJs = function() {
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
    };

    instance.withTimezone = function(dayjsInstance, timezone) {
        const dayjs = this.ensureDayJs();

        if (timezone && dayjs.tz) {
            return dayjsInstance.tz(timezone);
        }

        return dayjsInstance;
    };

    instance.getNow = function(timezone) {
        const dayjs = this.ensureDayJs();

        if (timezone && dayjs.tz) {
            return dayjs().tz(timezone);
        }

        return dayjs();
    };

    instance.normalizeInterval = function(intervalMs) {
        const parsed = Number(intervalMs);

        if (!Number.isFinite(parsed) || parsed <= 0) {
            return 1000;
        }

        return Math.max(50, parsed);
    };

    instance.recomputeBaseInterval = function() {
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
    };

    instance.restartTimer = function() {
        if (this.timerId) {
            clearTimeout(this.timerId);
            this.timerId = null;
        }

        if (this.subscribers.size > 0) {
            this.scheduleNextTick();
        }
    };

    instance.startTimer = function() {
        if (this.timerId) {
            return;
        }

        this.scheduleNextTick();
    };

    instance.stopTimerIfIdle = function() {
        if (this.subscribers.size === 0 && this.timerId) {
            clearTimeout(this.timerId);
            this.timerId = null;
        }
    };

    instance.safeUpdate = function(sub, value) {
        try {
            sub.dotNetRef.invokeMethodAsync("OnUpdate", value);
        } catch (error) {
            console.error("Dayjs subscription update failed", error);
        }
    };

    instance.computeValue = function(sub) {
        const dayjs = this.ensureDayJs();

        switch (sub.type) {
            case "relative": {
                const target = this.withTimezone(dayjs(sub.value), sub.timezone);
                const base = this.getNow(sub.timezone);
                return target.from(base, sub.withoutSuffix);
            }
            default:
                return "";
        }
    };

    instance.publish = function(sub) {
        const value = this.computeValue(sub);
        this.safeUpdate(sub, value);
    };

    instance.scheduleNextTick = function() {
        if (this.subscribers.size === 0) {
            return;
        }

        const now = (typeof performance !== "undefined" && performance.now) ? performance.now() : Date.now();
        const base = this.baseIntervalMs;
        const delay = base - (now % base);

        this.timerId = setTimeout(this.tick, delay);
    };

    instance.tick = function() {
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
    };

    instance.onVisibilityChange = function() {
        if (document.visibilityState !== "visible") {
            return;
        }

        const now = Date.now();
        for (const sub of this.subscribers.values()) {
            sub.lastTick = now;
            this.publish(sub);
        }
    };

    instance.createSubscription = function(subscriber) {
        const id = this.nextId++;
        const intervalMs = this.normalizeInterval(subscriber.intervalMs);

        const sub = {
            id,
            type: subscriber.type,
            timezone: subscriber.timezone,
            value: subscriber.value,
            withoutSuffix: subscriber.withoutSuffix === true,
            intervalMs,
            lastTick: 0,
            dotNetRef: subscriber.dotNetRef
        };

        this.subscribers.set(id, sub);
        this.recomputeBaseInterval();
        this.startTimer();
        this.publish(sub);

        return id;
    };

    instance.init = function() {
        this.ensureDayJs();
    };

    instance.fromNow = function(value, withoutSuffix, timezone) {
        const dayjs = this.ensureDayJs();
        const target = this.withTimezone(dayjs(value), timezone);
        const base = this.getNow(timezone);
        return target.from(base, withoutSuffix === true);
    };

    instance.toNow = function(value, withoutSuffix, timezone) {
        const dayjs = this.ensureDayJs();
        const target = this.withTimezone(dayjs(value), timezone);
        const base = this.getNow(timezone);
        return target.to(base, withoutSuffix === true);
    };

    instance.durationHumanize = function(amountMs, withoutSuffix) {
        const dayjs = this.ensureDayJs();
        return dayjs.duration(amountMs).humanize(withoutSuffix === true);
    };

    instance.subscribeRelative = function(value, intervalMs, withoutSuffix, timezone, dotNetRef) {
        return this.createSubscription({
            type: "relative",
            value,
            intervalMs,
            withoutSuffix,
            timezone,
            dotNetRef
        });
    };

    instance.unsubscribe = function(id) {
        this.subscribers.delete(id);
        this.recomputeBaseInterval();
        this.stopTimerIfIdle();
    };

        instance.subscribers = new Map();
        instance.timerId = null;
        instance.baseIntervalMs = 1000;
        instance.nextId = 1;
        instance.initialized = false;
        instance.tick = instance.tick.bind(this);
        instance.onVisibilityChange = instance.onVisibilityChange.bind(this);

        if (typeof document !== "undefined") {
            document.addEventListener("visibilitychange", instance.onVisibilityChange);
        }
    

    return instance;
})();
export function fromNow(value, withoutSuffix, timezone) {
    return interop.fromNow(value, withoutSuffix, timezone);
}

export function toNow(value, withoutSuffix, timezone) {
    return interop.toNow(value, withoutSuffix, timezone);
}

export function durationHumanize(durationMs, withoutSuffix) {
    return interop.durationHumanize(durationMs, withoutSuffix);
}

export function subscribeRelative(value, intervalMs, withoutSuffix, timezone, dotNetRef) {
    return interop.subscribeRelative(value, intervalMs, withoutSuffix, timezone, dotNetRef);
}

export function unsubscribe(id) {
    return interop.unsubscribe(id);
}
