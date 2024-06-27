import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import eventBus from "./eventBus";

type Callback = (...arg: any[]) => void;

class SignalRHub {
  constructor() {
    this._client = new HubConnectionBuilder()
      .withUrl("https://api.r4d4.work/playerHub", { withCredentials: true })
      // .withUrl("http://localhost:8082/playerHub", { withCredentials: true })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Error)
      .build();

    this._client.onclose(this.onDisconnected.bind(this));
    this._client.onreconnected(this.onReconnected.bind(this));
  }

  public start(): void {
    this._closingNormally = false;
    this._client
      .start()
      .then(() => this.onConnected())
      .catch((error) => {
        const errorType = "Not able to connect to backend with SignalR!";
        console.error(errorType, error);
        eventBus.emit("error", errorType, error);
      });
  }

  public stop(): void {
    this._closingNormally = true;
    this._client
      .stop()
      .then(() => this.onDisconnected())
      .catch((error) => {
        console.error("Error stopping SignalR connection: ", error);
      });
  }

  public invoke(methodeName: string, ...argument: any[]): void {
    if (this._isConnected) this._client.invoke(methodeName, ...argument);
  }

  public on(methodeName: string, callback: Callback): void {
    this._client.on(methodeName, callback);
  }

  public off(methodeName: string, callback: Callback): void {
    this._client.off(methodeName, callback);
  }

  public invokeWithNoData(methodeName: string): void {
    this._client.invoke(methodeName);
  }

  public isConnected(): boolean {
    return this._isConnected;
  }

  private onReconnected(connectionId?: string | undefined): void {
    this._isConnected = true;
    eventBus.emit("signalr-connected", true);
    console.log("SignalR reconneected to server!");
  }

  private onConnected(): void {
    this._isConnected = true;
    eventBus.emit("signalr-connected", true);
  }

  private onDisconnected(error?: Error | undefined): void {
    this._isConnected = false;
    if (this._closingNormally) {
      return;
    }
    eventBus.emit("signalr-connected", false);
    eventBus.emit("error", "Lost connection to backen via SignalR!", "Try again by reloading");
  }

  private _isConnected: boolean = false;
  private _client: HubConnection;
  private _closingNormally: boolean = false;
}

export default new SignalRHub();
