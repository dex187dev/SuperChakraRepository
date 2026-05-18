const { createApp } = Vue;

const app = createApp({

    data() {
        return { 
            activeView: 'meta',
            showAnalysis: false,
            showGraphics: false,
            chakraData: [],
            analysisResults: [],
            version: '1.3.6'
        }
    },

    methods: {

        switchTable(view) {
            this.activeView = view;
            this.sendToApp('refresh');
        },

        dbAction(command) {
            const msg = command.includes('drop') ? "WARNING: Delete complete DB?" : "Initialize DB?";
            if (confirm(msg))
                this.sendToApp(command);
        },

        moduleAction(command) { 
            this.sendToApp(command);
        },

        sqlAction(command) {
            this.sendToApp(command);
        },

        sendToApp(command) { 
            if (window.chrome && window.chrome.webview) {
                window.chrome.webview.postMessage({
                    command: command,
                    view: this.activeView
                });
            }
        },

        renderTable(data) { 

            if (Array.isArray(data)) {
                this.chakraData = data.map(item => {
                    const normalized = {};
                    for (let key in item) {
                        const normalizedKey = key.charAt(0).toUpperCase() + key.slice(1);
                        normalized[normalizedKey] = item[key];
                    }
                    if (normalized.Index === undefined) {
                        normalized.Index = item.index !== undefined ? item.index : Math.random();
                    }
                    return normalized;
                });
            } else {
                this.chakraData = data;
            }

        },

        displayAnalysisResults(results) {

            if (Array.isArray(results)) {
                this.analysisResults = results.map(res => {
                    const normalized = {};
                    for (let key in res) {
                        const normalizedKey = key.charAt(0).toUpperCase() + key.slice(1);
                        normalized[normalizedKey] = res[key];
                    }
                    return normalized;
                });
            } else {
                this.analysisResults = results;
            }
        },

        setAppVersion(v) {
            this.version = v;
        }

    },

    mounted() {
        console.log("SuperChakra Dashboard: Vue Core loaded.");
    }

});

window.vueApp = app;

const vm = app.mount('#app');

window.renderTable = (rawData) => {
    try {
        const data = typeof rawData === 'string' ? JSON.parse(rawData) : rawData;
        vm.renderTable(data);
    } catch (e) {
        console.error("Fehler bei window.renderTable:", e);
    }
};

window.displayAnalysisResults = (rawResults) => {
    try {
        const results = typeof rawResults === 'string' ? JSON.parse(rawResults) : rawResults;
        vm.displayAnalysisResults(results);
    } catch (e) {
        console.error("Fehler bei window.displayAnalysisResults:", e);
    }
};

window.setAppVersion = (v) => vm.setAppVersion(v);
window.switchTable = (view) => vm.switchTable(view);

