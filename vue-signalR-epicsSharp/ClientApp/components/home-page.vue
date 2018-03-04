<template>
    <div>
        <h1>Monitoring</h1>
        <!--<h2>{{processVariable.name}} = {{processVariable.value}}</h2>-->
        <p>Server time is: {{ticker}}</p>
    </div>
</template>
<script>
    export default {
        data() {
            return {
                connection: null,
                processVariable: null,
                ticker: null
            }
        },
        created: function () {
            this.connection = new this.$signalR.HubConnection('/home');
        },
        mounted: function () {
            this.connection.start();

            this.connection.on('minitoredPV', data => {
                this.processVariable = data;
            });

            this.connection.on('fastTick', data => {
                this.ticker = data;
            });

        }
    }
</script>
<style>
</style>
