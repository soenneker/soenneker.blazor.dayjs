const subscribers = new Map();
let dayjsTimerId = null;
let baseIntervalMs = 1000;
let nextSubscriptionId = 1;
let dayjsInitialized = false;

function ensureDayJs() {
    const dayjs = window.dayjs;

    if (!dayjs) {
        throw new Error("Day.js not found. Add dayjs + plugins to the page before using Soenneker.Blazor.Dayjs.");
    }

    if (!dayjsInitialized) {
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

        dayjsInitialized = true;
    }

    return dayjs;
}

function withTimezone(dayjsInstance, timezone) {
    const dayjs = ensureDayJs();

    if (timezone && dayjs.tz) {
        return dayjsInstance.tz(timezone);
    }

    return dayjsInstance;
}

function getNow(timezone) {
    const dayjs = ensureDayJs();

    if (timezone && dayjs.tz) {
        return dayjs().tz(timezone);
    }

    return dayjs();
}

function normalizeInterval(intervalMs) {
    const parsed = Number(intervalMs);

    if (!Number.isFinite(parsed) || parsed <= 0) {
        return 1000;
    }

    return Math.max(50, parsed);
}

function recomputeBaseInterval() {
    let min = 1000;

    for (const sub of subscribers.values()) {
        if (sub.intervalMs < min) {
            min = sub.intervalMs;
        }
    }

    min = Math.max(50, min);

    if (min !== baseIntervalMs) {
        baseIntervalMs = min;
        restartTimer();
    }
}

function restartTimer() {
    if (dayjsTimerId) {
        clearTimeout(dayjsTimerId);
        dayjsTimerId = null;
    }

    if (subscribers.size > 0) {
        scheduleNextTick();
    }
}

function startTimer() {
    if (dayjsTimerId) {
        return;
    }

    scheduleNextTick();
}

function stopTimerIfIdle() {
    if (subscribers.size === 0 && dayjsTimerId) {
        clearTimeout(dayjsTimerId);
        dayjsTimerId = null;
    }
}

function safeUpdate(sub, value) {
    try {
        sub.dotNetRef.invokeMethodAsync("OnUpdate", value);
    } catch (error) {
        console.error("Dayjs subscription update failed", error);
    }
}

function computeValue(sub) {
    const dayjs = ensureDayJs();

    switch (sub.type) {
        case "relative": {
            const target = withTimezone(dayjs(sub.value), sub.timezone);
            const base = getNow(sub.timezone);
            return target.from(base, sub.withoutSuffix);
        }
        default:
            return "";
    }
}

function publish(sub) {
    const value = computeValue(sub);
    safeUpdate(sub, value);
}

function scheduleNextTick() {
    if (subscribers.size === 0) {
        return;
    }

    const now = (typeof performance !== "undefined" && performance.now) ? performance.now() : Date.now();
    const base = baseIntervalMs;
    const delay = base - (now % base);

    dayjsTimerId = setTimeout(() => tick(), delay);
}

function tick() {
    const now = Date.now();

    for (const sub of subscribers.values()) {
        const due = Math.floor(now / sub.intervalMs) * sub.intervalMs;
        if (due > sub.lastTick) {
            sub.lastTick = due;
            publish(sub);
        }
    }

    dayjsTimerId = null;
    scheduleNextTick();
}

function onVisibilityChange() {
    if (document.visibilityState !== "visible") {
        return;
    }

    const now = Date.now();
    for (const sub of subscribers.values()) {
        sub.lastTick = now;
        publish(sub);
    }
}

function createSubscription(subscriber) {
    const id = nextSubscriptionId++;
    const intervalMs = normalizeInterval(subscriber.intervalMs);

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

    subscribers.set(id, sub);
    recomputeBaseInterval();
    startTimer();
    publish(sub);

    return id;
}

export function fromNow(value, withoutSuffix, timezone) {
    const dayjs = ensureDayJs();
    const target = withTimezone(dayjs(value), timezone);
    const base = getNow(timezone);
    return target.from(base, withoutSuffix === true);
}

export function toNow(value, withoutSuffix, timezone) {
    const dayjs = ensureDayJs();
    const target = withTimezone(dayjs(value), timezone);
    const base = getNow(timezone);
    return target.to(base, withoutSuffix === true);
}

export function durationHumanize(amountMs, withoutSuffix) {
    const dayjs = ensureDayJs();
    return dayjs.duration(amountMs).humanize(withoutSuffix === true);
}

export function subscribeRelative(value, intervalMs, withoutSuffix, timezone, dotNetRef) {
    return createSubscription({
        type: "relative",
        value,
        intervalMs,
        withoutSuffix,
        timezone,
        dotNetRef
    });
}

export function unsubscribe(id) {
    subscribers.delete(id);
    recomputeBaseInterval();
    stopTimerIfIdle();
}

if (typeof document !== "undefined") {
    document.addEventListener("visibilitychange", onVisibilityChange);
}
