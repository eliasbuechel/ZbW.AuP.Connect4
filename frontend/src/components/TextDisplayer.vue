<template>
  <span>{{ displayingText }}</span>
</template>
<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "TextDisplayer",
  props: {
    text: {
      required: true,
      type: String,
    },
    maxCaracters: {
      required: true,
      type: Number,
    },
  },
  computed: {
    displayingText(): string {
      return this.text.length < this.maxCaracters ? this.text : this.text.slice(0, this.maxCaracters - 4) + " ...";
    },
  },
});
</script>
<style scoped>
span {
  overflow: hidden;
}
</style>
