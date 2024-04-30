<template>
  <div class="user-data">
    <label for="id">Id</label>
    <div>{{ identity.id }}</div>
    <label for="username">Username</label>
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
