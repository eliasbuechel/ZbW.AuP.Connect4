<template>
  <div class="error-view-container">
    <h1>{{ errorCode }}</h1>
    <span>{{ errorMessage }}</span>
  </div>
</template>
<script lang="ts">
import { PropType, defineComponent } from "vue";

export default defineComponent({
  name: "ErrorPageVue",
  props: {
    errorCode: {
      required: true,
      type: Object as PropType<String>,
    },
    errorMessage: {
      required: true,
      type: Object as PropType<String>,
    },
  },
});
</script>

<style>
.error-view-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 0 2rem;
  text-align: center;
}
</style>
