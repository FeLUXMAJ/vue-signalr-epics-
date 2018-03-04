<template>
    <div id="app" class="container-fluid">
        <div class="row">
            <div class="col-sm-9">
                <router-view></router-view>
            </div>
        </div>

    </div>

</template>

<script>
import Vue from 'vue'
import HomePage from './home-page'

Vue.component('home-page', HomePage);

export default {
    data() {
        return {
        }
    }
}
</script>

<style>
</style>
