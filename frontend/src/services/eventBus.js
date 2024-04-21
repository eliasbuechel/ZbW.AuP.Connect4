var EventBus = /** @class */ (function () {
    function EventBus() {
        this.listeners = new Map();
    }
    EventBus.prototype.emit = function (event) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        var eventListeners = this.listeners.get(event);
        if (eventListeners) {
            eventListeners.forEach(function (listener) {
                listener.apply(void 0, args);
            });
        }
    };
    EventBus.prototype.on = function (event, callback) {
        var eventListeners = this.listeners.get(event);
        if (!eventListeners) {
            eventListeners = new Set();
            this.listeners.set(event, eventListeners);
        }
        eventListeners.add(callback);
    };
    EventBus.prototype.off = function (event, callback) {
        var eventListeners = this.listeners.get(event);
        if (eventListeners) {
            eventListeners.delete(callback);
            if (eventListeners.size === 0) {
                this.listeners.delete(event);
            }
        }
    };
    return EventBus;
}());
var eventBus = new EventBus();
export default eventBus;
