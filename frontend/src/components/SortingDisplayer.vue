<template>
  <div class="sorting-arrows-container">
    <div class="arrow arrow-up" :class="{ active: sortingKey === currentSortingKey && sortingOrder === 'asc' }"></div>
    <div
      class="arrow arrow-down"
      :class="{ active: sortingKey === currentSortingKey && sortingOrder === 'desc' }"
    ></div>
  </div>
</template>
<script lang="ts">
import { defineComponent } from "vue";
export default defineComponent({
  name: "SortingDisplayer",
  props: {
    sortingKey: {
      required: true,
      type: String,
    },
    currentSortingKey: {
      required: true,
      type: String,
    },
    sortingOrder: {
      required: true,
      type: String,
    },
  },
});
</script>
<style scoped>
.sorting-arrows-container {
  display: flex;
  flex-direction: column;
  width: fit-content;
  gap: 0.3rem;
}
.arrow {
  opacity: 0.3;
  border-left: 0.4em solid transparent;
  border-right: 0.4em solid transparent;
}
.arrow-up {
  border-bottom: 0.6em solid var(--color-light);
}
.arrow-down {
  border-top: 0.6em solid var(--color-light);
}
.arrow.active {
  opacity: 1;
}
.arrow-up.active {
  border-bottom: 0.6em solid var(--color-orange);
}
.arrow-down.active {
  border-top: 0.6em solid var(--color-orange);
}
</style>
