<template>
  <div class="toggle-button">
    <label class="label">{{ label }}</label>
    <div :class="{ switch: true, active: state }" @click="toggle">
      <div class="slider"></div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ToggleButton",
  props: {
    label: {
      required: true,
      type: String,
    },
    state: {
      required: true,
      type: Boolean,
    },
  },
  methods: {
    toggle(): void {
      this.$emit("toggle-state", !this.state);
    },
  },
});
</script>

<style scoped>
.toggle-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

/* The switch - the box around the slider */
.switch {
  position: relative;
  display: block;
  width: 60px;
  height: 34px;
  background-color: var(--color-light);
  border-radius: 24px;
  cursor: pointer;
  margin-bottom: 0.5rem;
}

.slider {
  position: absolute;
  content: "";
  height: 26px;
  width: 26px;
  left: 4px;
  bottom: 4px;
  background-color: var(--color-orange);
  -webkit-transition: 0.4s;
  transition: 0.4s;
  border-radius: 24px;
}

.switch.active {
  background-color: var(--color-orange);
}

.switch.active > .slider {
  -webkit-transform: translateX(26px);
  -ms-transform: translateX(26px);
  transform: translateX(26px);
  background-color: var(--color-light);
}
</style>
