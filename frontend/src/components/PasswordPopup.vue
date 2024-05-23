<template>
    <div class="overlay" @click="closeIfClickedOutside">
        <div class="password-popup" ref="popup" @click.stop>
            <form class="password-change" @submit.prevent="changePassword">
                <div class="input-field">
                    <label for="password">old password</label>
                    <input type="password" id="old-password" v-model="credentials.oldPassword"
                        @focusout="validatePassword('old-password')" required />
                    <span v-if="errors.oldPassword" class="error">{{ errors.oldPassword }}</span>
                </div>
                <div class="input-field">
                    <label for="password">new password</label>
                    <input type="password" id="new-password" v-model="credentials.newPassword"
                        @focusout="validatePassword('new-password')" required />
                    <span v-if="errors.newPassword" class="error">{{ errors.newPassword }}</span>
                </div>
                <button class="button-submit" type="submit" :disabled="!allowPasswordChange">Change
                    password</button>
            </form>
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";



export default defineComponent({
    data() {
        return {
            credentials: {
                oldPassword: "",
                newPassword: ""
            },
            errors: {
                oldPassword: "",
                newPassword: "",
                general: ""
            }
        };
    },
    methods: {
        async changePassword() {
            try {
                const response = await this.$axios.post("/account/manage/info", this.credentials, { withCredentials: true })
                console.log("status:", response.status)
                if (response.status === 200) {
                    this.$emit("close")
                }
            } catch (error) {
                console.log("Change Password error:", error);
            }
        },
        async validatePassword(passwordField: String) {
            const passwordInput: HTMLInputElement = document.getElementById(passwordField.toString()) as HTMLInputElement;
            if (!passwordInput.checkValidity()) {
                this.errors.general = passwordInput.validationMessage;
                return;
            }
            this.errors.general = "";
            // missing validation logic for password
        },
        closeIfClickedOutside(event: MouseEvent) {
            const popup = this.$refs.popup as HTMLElement;
            const target = event.target as Node;
            if (popup && !popup.contains(target)) {
                this.$emit("close");
            }
        }
    },
    computed: {
        allowPasswordChange(): boolean {
            let oldPasswordIdValid: boolean = this.credentials.oldPassword.length != 0 && !this.errors.oldPassword;
            let newPasswordIdValid: boolean = this.credentials.newPassword.length != 0 && !this.errors.newPassword;
            return oldPasswordIdValid && newPasswordIdValid;
        },
    },
});
</script>


<style>
.overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.4);
    z-index: 999;
}

.password-popup {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 999;
    background-color: #df9500aa;
    padding: 2rem;
    border-radius: 1rem;
}

.password-popup-inner {
    display: flex;
    justify-content: center;
    align-items: center;
}

.password-change {
    display: flex;
    flex-direction: column;
    align-items: start;
}

.password-change>label {
    color: black;
    font-size: 0.8rem;
}

.password-change>input {
    border: none;
    background: none;
    border-bottom: 1px solid black;
    padding: 0.2rem;
    width: 13rem;
    margin: 0.5rem 0;
}

.password-change>button {
    margin-top: 1rem;
    padding: 0.5rem 1rem;

    border: 1px grey;
    border-radius: 0.5rem;
    cursor: pointer;
    transition: background-color 0.3s ease;
}
</style>
