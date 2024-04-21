type Listener = (...args: any[]) => void;

class EventBus {
  private listeners: Map<string, Set<Listener>>;

  constructor() {
    this.listeners = new Map();
  }

  emit(event: string, ...args: any[]): void {
    const eventListeners: Set<Listener> = this.listeners.get(event);

    if (eventListeners) {
      eventListeners.forEach((listener: Listener) => {
        listener(...args);
      });
    }
  }

  on(event: string, callback: Listener): void {
    let eventListeners: Set<Listener> = this.listeners.get(event);
    if (!eventListeners) {
      eventListeners = new Set();
      this.listeners.set(event, eventListeners);
    }
    eventListeners.add(callback);
  }

  off(event: string, callback: Listener): void {
    const eventListeners: Set<Listener> = this.listeners.get(event);
    if (eventListeners) {
      eventListeners.delete(callback);
      if (eventListeners.size === 0) {
        this.listeners.delete(event);
      }
    }
  }
}

const eventBus = new EventBus();
export default eventBus;
