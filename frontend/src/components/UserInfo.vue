<template>
  <div class="user-data" @click.self="closeDropdown">
    <div class="username" @click="toggleDropdown">{{ identity.username }}</div>
    <div v-if="dropdownVisible" class="dropdown-menu" @click.stop>
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
      if (this.dropdownVisible) {
        document.addEventListener("click", this.handleClickOutside);
      } else {
        document.removeEventListener("click", this.handleClickOutside);
      }
    },
    closeDropdown() {
      this.dropdownVisible = false;
      document.removeEventListener("click", this.handleClickOutside);
    },
    handleClickOutside(event: Event) {
      if (this.$el && !this.$el.contains(event.target as Node)) {
        this.closeDropdown();
      }
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
  color: #b27c0c;
  transition: 0.25s ease;
}

.dropdown-menu {
  color: #01172c;
  grid-row: 2;
  align-self: flex-start;
  width: fit-content;
}

.dropdown-menu ul {
  list-style: none;
  list-style-position: outside;
  padding: 0;
  margin: 0;
  background-color: #b27c0c;
  border-radius: 1em;
  margin-top: 0.5rem;
}

.dropdown-menu li {
  padding: 0.5em 1em;
  cursor: pointer;
}

.dropdown-menu li:hover {
  color: whitesmoke;
  transition: 0.25s ease;
}
</style>
