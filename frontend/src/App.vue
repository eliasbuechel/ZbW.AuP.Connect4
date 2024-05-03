<template>

  <body>
    <router-view></router-view>
  </body>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "App",
  components: {},
  mounted() {
    this.checkAuthOnReload();
  },
  methods: {
    async checkAuthOnReload() {
      try {
        const response = await this.$axios.post("account/checkAuthentication");
        if (!response.data.authenticated && this.$route.path !== '/login') {
          this.$router.push('/login');
        }
      } catch (error) {
        console.error("Fehler beim Überprüfen des Authentifizierungsstatus:", error);

        this.$router.push('/login');
      }
    }
  }
});
</script>

<style>
@import "@/assets/global.css";
</style>
