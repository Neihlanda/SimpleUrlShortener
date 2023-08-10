<template>
  <div class="navbar bg-primary text-primary-content">
    <div class="navbar-start">
      <NuxtLink to="/" class="btn btn-ghost normal-case text-xl" > SimpleURLShortener </NuxtLink>
    </div>

    <div class="navbar-end">
      <a v-if="isLoggedIn" class="btn" @click="account.logout()" >Se deconnecter</a>
    </div>

  </div>
</template>

<script setup>
const account = useAccountStore();
const isLoggedIn = computed(() => account.loggedIn);
</script>
