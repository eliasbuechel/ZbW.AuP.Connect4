<template>
  <div class="user-data">
    <div :class="{ username: true, usernameExpanded: dropdownVisible }" @click="toggleDropdown">
      {{ identity.username }}
    </div>
    <div v-if="dropdownVisible" class="user-actions-container">
      <form @submit.prevent="changePassword">
        <div class="input-field input-field-light">
          <label for="oldPassword">Old password</label>
          <input
            type="password"
            id="oldPassword"
            v-model="credentials.oldPassword"
            @focusout="validateOldPassword()"
            required
          />
          <span v-if="errors.oldPassword" class="error">{{ errors.oldPassword }}</span>
        </div>
        <div class="input-field input-field-light">
          <label for="newPassword">New password</label>
          <input
            type="password"
            id="newPassword"
            v-model="credentials.newPassword"
            @focusout="validateNewPassword()"
            required
          />
          <span v-if="errors.newPassword" class="error">{{ errors.newPassword }}</span>
        </div>
        <button type="submit" class="button-white password-button">Change password</button>
        <span v-if="errors.resetPassword" class="error align-right">{{ errors.resetPassword }}</span>
      </form>
      <button @click="logout" class="button-white">Logout</button>
    </div>
  </div>
</template>

<script lang="ts">
  import { PlayerIdentity } from "@/types/PlayerIdentity";
  import { PropType, defineComponent } from "vue";

  export default defineComponent({
    name: "UserInfoVue",
    props: {
      identity: {
        required: true,
        type: Object as PropType<PlayerIdentity>,
      },
    },
    data() {
      return {
        dropdownVisible: false,
        credentials: {
          oldPassword: "",
          newPassword: "",
        },
        errors: {
          oldPassword: "",
          newPassword: "",
          resetPassword: "",
        },
      };
    },
    methods: {
      toggleDropdown() {
        if (!this.dropdownVisible) {
          this.errors.oldPassword = "";
          this.errors.newPassword = "";
          this.errors.resetPassword = "";
          this.credentials.oldPassword = "";
          this.credentials.newPassword = "";
        }

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
      async changePassword() {
        try {
          const response = await this.$axios.post("/account/manage/info", this.credentials, {
            withCredentials: true,
          });
          if (response.status === 200) {
            this.errors.resetPassword = "";
          }
        } catch (error) {
          this.errors.resetPassword = "Invalid password!";
        }

        this.credentials.oldPassword = "";
        this.credentials.newPassword = "";
      },
      async validateOldPassword() {
        const passwordInput: HTMLInputElement = document.getElementById("oldPassword") as HTMLInputElement;
        if (!passwordInput.checkValidity()) {
          this.errors.oldPassword = passwordInput.validationMessage;
          return;
        }
        this.errors.oldPassword = "";
        // missing validation logic for password
      },
      async validateNewPassword() {
        const passwordInput: HTMLInputElement = document.getElementById("newPassword") as HTMLInputElement;
        if (!passwordInput.checkValidity()) {
          this.errors.newPassword = passwordInput.validationMessage;
          return;
        }
        this.errors.newPassword = "";
        // missing validation logic for password
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
    },
  });
</script>

<style scoped>
  .user-data {
    color: var(--color-light);
    display: flex;
    flex-direction: column;
    align-items: center;
    border: 3px solid var(--color-orange);
    border-radius: 1rem;
    height: fit-content;
    transition: 0.2s ease-in-out;
    background-color: var(--color-dark);
  }

  .user-data:hover:enabled {
    cursor: pointer;
    transition: 0.2s ease-in-out;
  }

  .username {
    cursor: pointer;
    transition: 0.2s ease-in-out;
    padding: 1rem;
    width: 100%;
    text-align: center;
  }

  .usernameExpanded {
    color: var(--color-orange);
    transition: 0.2s ease-in-out;
  }

  .username:hover {
    color: var(--color-orange);
    transition: 0.2s ease-in-out;
  }

  .user-actions-container {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    padding: 0 1rem 1rem 1rem;
  }

  .align-right {
    align-self: flex-end;
  }

  form {
    display: flex;
    flex-direction: column;
  }

  form > button {
    align-self: stretch;
    margin-top: 0.5rem;
  }
</style>
