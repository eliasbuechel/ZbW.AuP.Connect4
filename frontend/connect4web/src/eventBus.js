class EventBus {
  constructor() {
    this.listeners = {};
  }

  emit(event, ...args) {
    if (this.listeners[event]) {
      this.listeners[event].forEach((listener) => {
        listener(...args);
      });
    }
  }

  on(event, callback) {
    if (!this.listeners[event]) {
      this.listeners[event] = [];
    }
    this.listeners[event].push(callback);
  }

  off(event, callback) {
    if (this.listeners[event]) {
      this.listeners[event] = this.listeners[event].filter(
        (listener) => listener !== callback
      );
    }
  }
}

const eventBus = new EventBus();

export default eventBus;
