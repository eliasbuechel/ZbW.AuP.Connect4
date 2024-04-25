<template>
  <div class="user-data">
    <label for="id">Id</label>
    <div>{{ identity.id }}</div>
    <label for="username">Username</label>
    <div>{{ identity.username }}</div>
  </div>
</template>

<script lang="ts">
import { PlayerIdentity } from "@/DataTransferObjects";
import eventBus from "@/services/eventBus";
import signalRHub from "@/services/signalRHub";
import { defineComponent } from "vue";

interface UserDataState {
  identity: PlayerIdentity;
  isSubscribed: boolean;
}

export default defineComponent({
  mounted() {
    if (!signalRHub.isConnected()) {
      this.subscribe();
      signalRHub.invoke("GetUserData");
    }

    eventBus.on("signalr-connected", this.onSignalRConnected);
    eventBus.on("signalr-disconnected", this.onSignalRDisconnected);
  },
  unmounted() {
    eventBus.off("signalr-connected", this.onSignalRConnected);
    eventBus.off("signalr-disconnected", this.onSignalRDisconnected);
  },
  data(): UserDataState {
    return {
      identity: { id: "", username: "" },
      isSubscribed: false,
    };
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      signalRHub.on("send-user-data", this.updateUserIdentity);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("send-user-data", this.updateUserIdentity);
    },
    updateUserIdentity(identity: PlayerIdentity): void {
      this.identity = identity;
    },
    onSignalRConnected(): void {
      this.subscribe();
      signalRHub.invoke("GetUserData");
    },
    onSignalRDisconnected(): void {
      this.unsubscribe();
    },
  },
});
</script>
