<template>
  <body>
    <ErrorPage v-if="errorCode != null && errorMessage != null" :errorCode="errorCode" :errorMessage="errorMessage" />
    <router-view v-else />
  </body>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import ErrorPage from "./views/ErrorPage.vue";
import eventBus from "./services/eventBus";

export default defineComponent({
  name: "App",
  data(): { errorCode?: string; errorMessage?: string } {
    return {
      errorCode: undefined,
      errorMessage: undefined,
    };
  },
  components: {
    ErrorPage,
  },
  mounted() {
    eventBus.on("error", this.onError);
  },
  unmounted() {
    eventBus.off("error", this.onError);
  },
  methods: {
    onError(errorCode: string, errorMessage: string): void {
      this.errorCode = errorCode;
      this.errorMessage = errorMessage;
    },
  },
});
</script>

<style>
@import "@/assets/global.css";
</style>
