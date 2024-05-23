<template>
  <div class="user-data">
    <div class="username" @click="toggleDropdown">{{ identity.username }}</div>
    <div v-if="dropdownVisible" class="dropdown-menau" @click.stop>
      <ul>
        <li @click="openPasswordPopup">Change password</li>
        <li @click="logout">Logout</li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";
import eventBus from "@/services/eventBus";


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
      password: {
        old: "",
        new: ""
      },
    };
  },
  methods: {
    toggleDropdown() {
      this.dropdownVisible = !this.dropdownVisible;
    },
    openPasswordPopup() {
      eventBus.emit("open-password-popup");
      this.dropdownVisible = false;
    },
    async logout() {
      try {
        await this.$axios.post("/account/logout?useCookies=true", {}, { withCredentials: true });
        this.$router.push("/login");
        this.dropdownVisible = false;
      } catch (error) {
        console.log("Logout failed:", error);
      }
    },
  }
});
</script>

<style scoped>
.username {
  cursor: pointer;
}

.username:hover {
  color: #df9500aa;
  transition: 0.25s ease;
}

.dropdown-menu {
  display: flex;
  justify-content: left;
}

.dropdown-menu ul {
  list-style: none;
  list-style-position: outside;
  text-align: left;
  padding: 0;
  margin: 0;
  background-color: #df95003c;
}

.dropdown-menu li {
  padding: 0.3em;
  cursor: pointer;
}

.dropdown-menu li:hover {
  color: #df9500aa;
  transition: 0.25s ease;
}
</style>
