type Callback = (...args: any[]) => void;

class EventBus {
  public emit(event: string, ...args: any[]): void {
    const callbacks: Set<Callback> | undefined = this._eventCallbackMap.get(event);

    if (!callbacks) return;

    callbacks.forEach((listener: Callback) => {
      listener(...args);
    });
  }

  public on(event: string, callback: Callback): void {
    let callbacks: Set<Callback> | undefined = this._eventCallbackMap.get(event);
    if (!callbacks) {
      callbacks = new Set();
      this._eventCallbackMap.set(event, callbacks);
    }
    callbacks.add(callback);
  }

  public off(event: string, callback: Callback): void {
    const eventListeners: Set<Callback> | undefined = this._eventCallbackMap.get(event);
    if (eventListeners) {
      eventListeners.delete(callback);
      if (eventListeners.size === 0) {
        this._eventCallbackMap.delete(event);
      }
    }
  }

  private _eventCallbackMap: Map<string, Set<Callback>> = new Map<string, Set<Callback>>();
}

const eventBus = new EventBus();
export default eventBus;
