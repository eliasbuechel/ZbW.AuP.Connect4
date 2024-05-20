<template>
  <div class="user-data" @click="toggleDropdown">
    <div class="username">{{ identity.username }}</div>
    <div v-if="dropdownVisible" class="dropdown-menu">
      <button @click="changePasswort">Change password</button>
      <button @click="logout">Logout</button>
    </div>
  </div>
</template>

<script lang="ts">
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  props: {
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  data() {
    return {
      dropdownVisible: false,
    };
  },
  mounted() {
    document.addEventListener('click', this.handleClickOutside);
  },
  beforeUnmount() {
    document.removeEventListener('click', this.handleClickOutside);
  },
  methods: {
    toggleDropdown() {
      this.dropdownVisible = !this.dropdownVisible;
    },
    changePasswort() {
      try {
        //
        this.dropdownVisible = false;
      } catch (error) {
        console.log("Change Passwort error:", error);
      }
    },
    async logout() {
      try {
        await this.$axios.post("/account/logout");
        this.$router.push("/login");
        this.dropdownVisible = false;
      } catch (error) {
        console.log("Logout fail:", error);
      }
    },
    handleClickOutside(event: MouseEvent) {
      const target = event.target as HTMLElement;
      if (!this.$el.contains(target)) {
        this.dropdownVisible = false;
      }
    }
  }
});
</script>
