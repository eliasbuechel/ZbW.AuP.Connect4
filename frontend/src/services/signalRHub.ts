import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import eventBus from "./eventBus";

type Callback = (...arg: any[]) => void;

class SignalRHub {
  constructor() {
    this.client = new HubConnectionBuilder().withUrl("http://localhost:7136/playerHub").build();

    this.client.onreconnected(this.onReconnected.bind(this));
    this.client.onclose(this.onDisconnected.bind(this));
  }

  public start(): void {
    this.client
      .start()
      .then(() => this.onConnected())
      .catch((error) => {
        console.error("Error starting SignalR connection: ", error);
        setTimeout(() => this.start(), 5000);
      });
  }

  public stop(): void {
    this.client
      .stop()
      .then(() => this.onDisconnected())
      .catch((error) => {
        console.error("Error stopping SignalR connection: ", error);
      });
  }

  public invoke(methodeName: string, ...argument: any[]): void {
    if (this.isConnected) this.client.invoke(methodeName, ...argument);
  }

  public on(methodeName: string, callback: Callback): void {
    this.client.on(methodeName, callback);
  }

  public invokeWithNoData(methodeName: string): void {
    this.client.invoke(methodeName);
  }

  private onReconnected(connectionId?: string | undefined): void {
    this.isConnected = true;
    eventBus.emit("signalr-connected", true);
    console.log("SignalR reconneected to server ", connectionId);
  }

  private onConnected(): void {
    this.isConnected = true;
    eventBus.emit("signalr-connected", true);
    console.log("SignalR connected to server");
  }

  private onDisconnected(error?: Error | undefined): void {
    this.isConnected = false;
    console.log("SignalR disconnected");
    eventBus.emit("signalr-connected", false);
    this.start();
  }

  private isConnected: boolean = false;
  private client: HubConnection;
}

export default new SignalRHub();
