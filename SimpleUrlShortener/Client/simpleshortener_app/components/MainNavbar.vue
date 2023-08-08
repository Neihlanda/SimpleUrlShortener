<template>
  <div class="navbar bg-primary text-primary-content">
    <div class="navbar-start">
      <a class="btn btn-ghost normal-case text-xl">SimpleURLShortener</a>
    </div>

    <div class="navbar-end">
      <a v-if="isLoggedIn" class="btn" @click="logOut" >Se deconnecter</a>
    </div>

  </div>
</template>

<script setup>

const isLoggedIn = ref(false)

function logOut(){
//TODO: call auth logout
}

</script>
