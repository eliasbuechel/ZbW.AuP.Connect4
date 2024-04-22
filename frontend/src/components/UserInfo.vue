<template>
  <div class="user-data">
    <label for="id">Id</label>
    <div>{{ identity.id }}</div>
    <label for="username">Username</label>
    <div>{{ identity.username }}</div>
  </div>
</template>

<script lang="ts">
import eventBus from "@/services/eventBus";
import signalRHub from "@/services/signalRHub";
import { defineComponent } from "vue";

interface UserIdentity {
  id: string;
  username: string;
}

export default defineComponent({
  mounted() {
    signalRHub.on("send-user-data", this.updateUserIdentity);

    if (!signalRHub.isConnected()) eventBus.on("signalr-connected", this.onSignalRConnected);
    else signalRHub.invoke("GetUserData");
  },
  unmounted() {
    signalRHub.off("send-user-data", this.updateUserIdentity);
  },
  data(): { identity: UserIdentity } {
    return {
      identity: { id: "", username: "" },
    };
  },
  methods: {
    updateUserIdentity(identity: UserIdentity): void {
      this.identity = identity;
    },
    onSignalRConnected(): void {
      signalRHub.invoke("GetUserData");
      eventBus.off("signalr-connected", this.onSignalRConnected);
    },
  },
});
</script>
