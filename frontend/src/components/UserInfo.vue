<template>
  <div class="user-data">
    <div>{{ identity.username }}</div>
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
});
</script>
