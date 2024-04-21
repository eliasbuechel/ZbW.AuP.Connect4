import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import eventBus from "./eventBus";

class SignalRHub {
  private client: HubConnection;
  private isConnected: boolean;

  constructor() {
    this.client = new HubConnectionBuilder()
      .withUrl("http://localhost:7136/playerHub")
      .build();

    this.client.onreconnected(this.onReconnected.bind(this));
    this.client.onclose(this.onDisconnected.bind(this));
    this.isConnected = false;
  }

  start(): void {
    this.client
      .start()
      .then(() => this.onConnected())
      .catch((error: Error) => {
        console.error("Error starting SignalR connection: ", error);
        setTimeout(() => this.start(), 5000);
      });
  }

  stop(): void {
    this.client
      .stop()
      .then(() => this.onDisconnected())
      .catch((error: Error) => {
        console.error("Error stopping SignalR connection: ", error);
      });
  }

  invoke(methodeName: string, ...argument: any[]): void {
    if (this.isConnected) this.client.invoke(methodeName, ...argument);
  }

  on(methodeName: string, callback: (...args: any[]) => void): void {
    this.client.on(methodeName, callback);
  }

  invokeWithNoData(methodeName: string): void {
    this.client.invoke(methodeName);
  }

  onReconnected(connectionId: string): void {
    this.isConnected = true;
    eventBus.emit("signalr-connected", true);
    console.log("SignalR reconneected to server ", connectionId);
  }

  onConnected(): void {
    this.isConnected = true;
    eventBus.emit("signalr-connected", true);
    console.log("SignalR connected to server");
  }

  onDisconnected(): void {
    this.isConnected = false;
    console.log("SignalR disconnected");
    eventBus.emit("signalr-connected", false);
    this.start();
  }
}

export default new SignalRHub();
