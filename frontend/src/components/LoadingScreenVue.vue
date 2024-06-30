<template>
  <div class="loading-screen-text">Loading...</div>
</template>
<script>
  import { defineComponent } from "vue";
  export default defineComponent({
    name: "LoadingScreenVue",
  });
</script>

<style scoped>
  .loading-screen-text {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
  }
</style>
