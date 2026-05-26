const { createApp } = Vue;

const app = createApp({

    data() {
        return { 
            activeView: 'meta',
            showAnalysis: false,
            showGraphics: false,
            chakraData: [],
            analysisResults: [],
            version: '1.3.8',
            arbeitstitel: 'Semesterarbeit',

            calcRows: [],
            calcPhysicsRows: [],
            showPhysicsTable: true,
            showBinaryTable: true,

            chartDichte: null,
            chartDrehimpuls: null,
            chartRadar: null,

            chakraFarben: ['#FF0000', '#FF8C00', '#FFFF00', '#00FF00', '#00BFFF', '#BF00FF', '#FF00FF'],

            calcPanelFarben: [
                '#333333', '#FF00FF', '#BF00FF', '#00BFFF', '#00FF00', '#FFFF00', '#FF8C00', '#FF0000',

                '#333333', '#FF00FF', '#BF00FF', '#00BFFF', '#00FF00', '#FFFF00', '#FF8C00', '#FF0000'
            ]

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

        toggleGraphics() {
            this.showGraphics = !this.showGraphics;

            if (this.showGraphics) {
                this.$nextTick(() => {
                    try {
                        if (this.chartDichte && typeof this.chartDichte.destroy === 'function') this.chartDichte.destroy();
                        if (this.chartDrehimpuls && typeof this.chartDrehimpuls.destroy === 'function') this.chartDrehimpuls.destroy();
                        if (this.chartRadar && typeof this.chartRadar.destroy === 'function') this.chartRadar.destroy();

                        this.initApexCharts();

                        this.sendToApp('request_graphics_data');
                    } catch (error) {
                        console.error("Fehler beim Neuaufbau der Charts im DOM:", error);
                    }
                });
            }
        },

        initApexCharts() {
            const chartConfig = {
                theme: { mode: 'dark', palette: 'palette1' },
                chart: { background: 'transparent', foreColor: '#f5f5f5', fontFamily: 'Segoe UI, sans-serif' },
                grid: { borderColor: '#333' }
            };

            // 1. Dichte - Kuchendiagramm
            this.chartDichte = new ApexCharts(document.querySelector("#chart-dichte"), {
                theme: { mode: 'dark' },
                chart: { background: 'transparent', foreColor: '#f5f5f5', fontFamily: 'Segoe UI, sans-serif', type: 'pie', height: 320 },
                grid: { borderColor: '#333' },

                series: [],
                labels: [],
                title: { text: 'Dichte-Verteilung', align: 'center' },
                legend: { position: 'bottom' },

                colors: [
                    '#FF0000',
                    '#FF5C00',
                    '#FFF500',
                    '#00FF00',
                    '#00FFFF',
                    '#0000FF',
                    '#FF00FF'
                ]
            });
            this.chartDichte.render();

            // 2. Drehimpuls - Liniendiagramm
            this.chartDrehimpuls = new ApexCharts(document.querySelector("#chart-drehimpuls"), {
                ...chartConfig,
                chart: {
                    ...chartConfig.chart,
                    type: 'line',
                    height: 320,
                    toolbar: { show: false },
                    grid: {
                        padding: {
                            bottom: 20
                        }
                    }
                },
                stroke: { curve: 'smooth', width: 4 },
                series: [{ name: 'Drehimpuls', data: [] }],
                xaxis: {
                    type: 'category',
                    categories: [],
                    labels: {
                        show: true,
                        rotate: -45,
                        style: { colors: '#f5f5f5', fontSize: '12px' }
                    }
                },
                yaxis: {
                    logarithmic: false,
                    forceNiceScale: true,
                    labels: {
                        show: true,
                        style: { colors: '#f5f5f5' },
                        formatter: (value) => {
                            if (value === 0) return '0';
                            if (Math.abs(value) >= 10000 || Math.abs(value) < 0.1) {
                                return Number(value).toExponential(1);
                            }
                            return value;
                        }
                    }
                },
                title: { text: 'Drehimpuls-Verlauf', align: 'center' },
                colors: ['#00E396']
            });
            this.chartDrehimpuls.render();

            // 3. Energie - Radar-Diagramm
            this.chartRadar = new ApexCharts(document.querySelector("#chart-radar"), {
                ...chartConfig,
                chart: { ...chartConfig.chart, type: 'radar', height: 320 },
                series: [{ name: 'Energie-Level', data: [] }],
                labels: [],
                title: { text: 'Energie-Harmonie', align: 'center' },
                colors: ['#FF4560'],
                fill: { opacity: 0.4 }
            });
            this.chartRadar.render();
        },

        updateApexCharts(data) {
            const labels = data.map(c => c.name || c.Name);
            const dichteWerte = data.map(c => c.dichte || c.Dichte);
            const drehimpulsWerte = data.map(c => c.drehimpuls || c.Drehimpuls);
            const energieWerte = data.map(c => c.energie || c.Energie);

            if (!this.chartDichte || !this.chartDrehimpuls || !this.chartRadar) {
                setTimeout(() => this.updateApexCharts(data), 100);
                return;
            }

            try {
                // Kuchendiagramm füttern
                this.chartDichte.updateOptions({ labels: labels, series: dichteWerte });

                // Liniendiagramm füttern: Wir übergeben X-Achse und Y-Werte im SELBEN Atemzug
                this.chartDrehimpuls.updateOptions({
                    xaxis: { categories: labels }
                });
                this.chartDrehimpuls.updateSeries([{
                    name: 'Drehimpuls',
                    data: drehimpulsWerte
                }]);

                // Radar-Diagramm füttern
                this.chartRadar.updateOptions({ labels: labels });
                this.chartRadar.updateSeries([{ name: 'Energie-Level', data: energieWerte }]);

            } catch (error) {
                console.error("Fehler beim Befüllen der frisch gebauten Charts:", error);
            }
        },

        isMaxValue(currentValue, columnName) {

            if (!this.chakraData || this.chakraData.length === 0) return false;

            const columnValues = this.chakraData.map(item => Number(item[columnName] || 0));
            const maxValue = Math.max(...columnValues);

            return Number(currentValue || 0) === maxValue;
        },

        getPhasenverschiebungClass(currentValue) {
            if (!this.chakraData || this.chakraData.length === 0) return '';

            const currentStr = Number(currentValue || 0).toFixed(7);

            if (currentStr === "0.0000000") return '';

            const matches = this.chakraData.filter(item => {
                const itemStr = Number(item.Phasenverschiebung || item.phasenverschiebung || 0).toFixed(7);
                return itemStr === currentStr;
            });

            if (matches.length > 1) {
                if (currentStr === "0.0067858") {
                    return 'highlight-phase-pair-1';
                }
                if (currentStr === "0.0220540") {
                    return 'highlight-phase-pair-2';
                }
                return 'highlight-phase-pair-1';
            }
            return '';
        },

        isBitActive(binaryString, position) {
            if (!binaryString) return false;

            const cleanStr = String(binaryString).replace(/\s+/g, '');
            const targetChar = cleanStr.charAt(cleanStr.length - 1 - position);

            return targetChar === '1';
        },

        calculatePhysics() {
            this.showPhysicsTable = true;
            this.sendToApp("calc_physics");
        },
        hidePhysics() {
            this.showPhysicsTable = false;
        },
        calculateBinary() {
            this.showBinaryTable = true;
            this.sendToApp("calc_binary");
        },
        hideBinary() {
            this.showBinaryTable = false;
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

window.renderChakraCharts = (rawChartData) => {
    try {

        console.log("===> WPF-BRÜCKE AUSGELÖST! Rohe Daten empfangen:", rawChartData);
        const data = typeof rawChartData === 'string' ? JSON.parse(rawChartData) : rawChartData;
        console.log("===> Geparste Daten für ApexCharts:", data);
        vm.updateApexCharts(data);

    } catch (e) {
        console.error("Kritischer Fehler bei window.renderChakraCharts:", e);
    }
};

//window.updateCalcPanel = (safeJsonData) => {
//    try {
//        const data = typeof safeJsonData === 'string' ? JSON.parse(safeJsonData) : safeJsonData;
//        vm.calcRows = data;
//    } catch (e) {
//        console.error("Fehler bei window.updateCalcPanel:", e);
//    }
//};

window.updatePhysicsOnlyBase64 = function (base64Str) {
    try {
        let jsonStr = decodeURIComponent(escape(window.atob(base64Str)));
        let data = JSON.parse(jsonStr);
        vm.calcPhysicsRows = data.physicsData;
    } catch (e) { console.error("Physics-Parsing-Fehler:", e); }
};

window.updateBinaryOnlyBase64 = function (base64Str) {
    try {
        let jsonStr = decodeURIComponent(escape(window.atob(base64Str)));
        let data = JSON.parse(jsonStr);
        vm.calcRows = data.calcData;
    } catch (e) { console.error("Binary-Parsing-Fehler:", e); }
};

window.updateAppMetadata = function (jsonStr) {
    try {
        let data = JSON.parse(jsonStr);

        vm.version = data.version;
        vm.arbeitstitel = data.arbeitstitel;
    }
    catch (error) {
        console.error("Fehler beim Laden der App-Metadaten:", error);
    }
};

