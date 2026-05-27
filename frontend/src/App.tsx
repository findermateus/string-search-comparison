import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer
} from 'recharts';
import { Upload, Search, BarChart3, Clock, Cpu, FileText, AlertCircle } from 'lucide-react';
import type { SearchResult, AlgorithmInfo } from './types';

const API_BASE = 'http://localhost:5000/api';

function App() {
  const [text, setText] = useState('');
  const [pattern, setPattern] = useState('');
  const [algorithm, setAlgorithm] = useState('');
  const [algorithms, setAlgorithms] = useState<AlgorithmInfo[]>([]);
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchAlgorithms();
  }, []);

  const fetchAlgorithms = async () => {
    try {
      const res = await axios.get(`${API_BASE}/algorithms`);
      setAlgorithms(res.data);
      if (res.data.length > 0) setAlgorithm(res.data[0].algorithmName);
    } catch (err) {
      console.error("Failed to fetch algorithms", err);
      setError("Não foi possível conectar à API do backend.");
    }
  };

  const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (event) => {
      setText(event.target?.result as string);
    };
    reader.readAsText(file);
  };

  const runSearch = async () => {
    if (!text || !pattern) return;
    setLoading(true);
    setError(null);
    try {
      const res = await axios.post(`${API_BASE}/search`, {
        text,
        pattern,
        algorithm,
        stepByStep: false
      });
      setResults([res.data]);
    } catch (err) {
      setError("Falha na execução da busca.");
    } finally {
      setLoading(false);
    }
  };

  const runCompare = async () => {
    if (!text || !pattern) return;
    setLoading(true);
    setError(null);
    try {
      const res = await axios.post(`${API_BASE}/compare`, { text, pattern });
      setResults(res.data);
    } catch (err) {
      setError("Falha na comparação.");
    } finally {
      setLoading(false);
    }
  };

  const chartData = results.map(r => ({
    name: r.algorithmName,
    time: r.elapsedNanoseconds / 1000,
    comparisons: r.comparisons
  }));

  return (
    <div className="min-h-screen bg-zinc-900 text-zinc-100 p-8 font-sans">
      <header className="max-w-6xl mx-auto mb-12 flex items-center justify-between">
        <div>
          <h1 className="text-4xl font-bold bg-gradient-to-r from-blue-400 to-emerald-400 bg-clip-text text-transparent">
            Comparação de Busca de Strings
          </h1>
          <p className="text-zinc-400 mt-2">Observabilidade & Análise de Performance</p>
        </div>
        <div className="flex gap-4">
          <div className="flex items-center gap-2 bg-zinc-800 px-4 py-2 rounded-lg border border-zinc-700">
            <Cpu size={18} className="text-blue-400" />
            <span className="text-sm">4 Algoritmos</span>
          </div>
          <div className="flex items-center gap-2 bg-zinc-800 px-4 py-2 rounded-lg border border-zinc-700">
            <BarChart3 size={18} className="text-emerald-400" />
            <span className="text-sm">Pronto para OpenTelemetry</span>
          </div>
        </div>
      </header>

      <main className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-1 space-y-6">
          <section className="bg-zinc-800 p-6 rounded-xl border border-zinc-700 shadow-xl">
            <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
              <FileText size={20} className="text-blue-400" /> Texto de Entrada
            </h2>
            
            <div className="mb-4">
              <label className="block text-sm text-zinc-400 mb-2">Carregar Arquivo (.txt)</label>
              <label className="flex flex-col items-center justify-center w-full h-32 border-2 border-zinc-600 border-dashed rounded-lg cursor-pointer bg-zinc-700 hover:bg-zinc-600 transition-colors">
                <div className="flex flex-col items-center justify-center pt-5 pb-6">
                  <Upload className="w-8 h-8 mb-3 text-zinc-400" />
                  <p className="text-xs text-zinc-400">Clique ou arraste e solte</p>
                </div>
                <input type="file" className="hidden" accept=".txt" onChange={handleFileUpload} />
              </label>
            </div>

            <div className="mb-4">
              <label className="block text-sm text-zinc-400 mb-2">Ou cole o texto manualmente</label>
              <textarea 
                className="w-full h-48 bg-zinc-900 border border-zinc-600 rounded-lg p-3 text-sm focus:ring-2 focus:ring-blue-500 outline-none resize-none"
                placeholder="Cole o texto aqui..."
                value={text}
                onChange={(e) => setText(e.target.value)}
              />
              <div className="text-[10px] text-zinc-500 mt-1 text-right">{text.length} caracteres</div>
            </div>
          </section>

          <section className="bg-zinc-800 p-6 rounded-xl border border-zinc-700 shadow-xl">
            <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
              <Search size={20} className="text-emerald-400" /> Parâmetros de Busca
            </h2>
            
            <div className="space-y-4">
              <div>
                <label className="block text-sm text-zinc-400 mb-2">Padrão de Busca (Pattern)</label>
                <input 
                  type="text"
                  className="w-full bg-zinc-900 border border-zinc-600 rounded-lg p-3 text-sm focus:ring-2 focus:ring-emerald-500 outline-none"
                  placeholder="Digite o padrão..."
                  value={pattern}
                  onChange={(e) => setPattern(e.target.value)}
                />
              </div>

              <div>
                <label className="block text-sm text-zinc-400 mb-2">Algoritmo</label>
                <select 
                  className="w-full bg-zinc-900 border border-zinc-600 rounded-lg p-3 text-sm focus:ring-2 focus:ring-emerald-500 outline-none"
                  value={algorithm}
                  onChange={(e) => setAlgorithm(e.target.value)}
                >
                  {algorithms.map(algo => (
                    <option key={algo.algorithmName} value={algo.algorithmName}>{algo.algorithmName}</option>
                  ))}
                </select>
              </div>

              <div className="pt-4 flex gap-3">
                <button 
                  onClick={runSearch}
                  disabled={loading || !text || !pattern}
                  className="flex-1 bg-blue-600 hover:bg-blue-500 disabled:bg-zinc-700 text-white font-bold py-3 rounded-lg transition-colors shadow-lg shadow-blue-900/20"
                >
                  {loading ? 'Executando...' : 'Buscar'}
                </button>
                <button 
                  onClick={runCompare}
                  disabled={loading || !text || !pattern}
                  className="flex-1 bg-emerald-600 hover:bg-emerald-500 disabled:bg-zinc-700 text-white font-bold py-3 rounded-lg transition-colors shadow-lg shadow-emerald-900/20"
                >
                  {loading ? 'Executando...' : 'Comparar Todos'}
                </button>
              </div>
            </div>
          </section>
          
          {error && (
            <div className="bg-red-900/30 border border-red-500/50 p-4 rounded-lg flex gap-3 text-red-200">
              <AlertCircle size={20} />
              <p className="text-sm">{error}</p>
            </div>
          )}
        </div>

        <div className="lg:col-span-2 space-y-8">
          {results.length > 0 && (
            <section className="bg-zinc-800 p-8 rounded-xl border border-zinc-700 shadow-2xl">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold flex items-center gap-3">
                  <BarChart3 size={24} className="text-emerald-400" /> Dashboard de Execução
                </h2>
                <div className="text-xs text-zinc-500 uppercase tracking-widest">Métricas de Performance em Tempo Real</div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="h-64">
                  <h3 className="text-sm text-zinc-400 mb-4 text-center">Tempo de Execução (μs)</h3>
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={chartData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#3f3f46" />
                      <XAxis dataKey="name" stroke="#a1a1aa" fontSize={10} hide={results.length === 1} />
                      <YAxis stroke="#a1a1aa" fontSize={10} />
                      <Tooltip 
                        contentStyle={{ backgroundColor: '#18181b', border: '1px solid #3f3f46', borderRadius: '8px' }}
                        itemStyle={{ color: '#60a5fa' }}
                      />
                      <Bar dataKey="time" fill="#3b82f6" radius={[4, 4, 0, 0]} name="Tempo (μs)" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>

                <div className="h-64">
                  <h3 className="text-sm text-zinc-400 mb-4 text-center">Comparações de Caracteres</h3>
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={chartData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#3f3f46" />
                      <XAxis dataKey="name" stroke="#a1a1aa" fontSize={10} hide={results.length === 1} />
                      <YAxis stroke="#a1a1aa" fontSize={10} />
                      <Tooltip 
                        contentStyle={{ backgroundColor: '#18181b', border: '1px solid #3f3f46', borderRadius: '8px' }}
                        itemStyle={{ color: '#34d399' }}
                      />
                      <Bar dataKey="comparisons" fill="#10b981" radius={[4, 4, 0, 0]} name="Comparações" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </section>
          )}

          {results.length > 0 && (
            <section className="bg-zinc-800 p-6 rounded-xl border border-zinc-700 shadow-xl overflow-hidden">
              <h2 className="text-xl font-semibold mb-6 flex items-center gap-2">
                <Clock size={20} className="text-blue-400" /> Detalhes da Comparação
              </h2>
              <div className="overflow-x-auto">
                <table className="w-full text-left text-sm">
                  <thead className="text-xs text-zinc-400 uppercase bg-zinc-900/50">
                    <tr>
                      <th className="px-4 py-3">Algoritmo</th>
                      <th className="px-4 py-3 text-right">Tempo (ns)</th>
                      <th className="px-4 py-3 text-right">Comparações</th>
                      <th className="px-4 py-3 text-right">Ocorrências</th>
                      <th className="px-4 py-3">Complexidade</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-zinc-700">
                    {results.sort((a,b) => a.elapsedNanoseconds - b.elapsedNanoseconds).map((r, i) => (
                      <tr key={r.algorithmName} className={i === 0 ? "bg-emerald-900/10" : ""}>
                        <td className="px-4 py-4 font-medium flex items-center gap-2">
                          {r.algorithmName}
                          {i === 0 && results.length > 1 && <span className="text-[10px] bg-emerald-500 text-white px-1.5 rounded uppercase">Mais Rápido</span>}
                        </td>
                        <td className="px-4 py-4 text-right tabular-nums text-zinc-300">
                          {r.elapsedNanoseconds.toLocaleString()}
                        </td>
                        <td className="px-4 py-4 text-right tabular-nums text-zinc-300">
                          {r.comparisons.toLocaleString()}
                        </td>
                        <td className="px-4 py-4 text-right tabular-nums text-zinc-300">
                          {r.positions.length}
                        </td>
                        <td className="px-4 py-4 font-mono text-xs text-zinc-500">
                          {r.shortComplexity}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </section>
          )}

          {!loading && results.length === 0 && (
            <div className="h-full min-h-[400px] flex flex-col items-center justify-center text-zinc-600 border-2 border-dashed border-zinc-800 rounded-3xl">
              <BarChart3 size={64} className="mb-4 opacity-20" />
              <p>Execute uma busca ou comparação para ver as métricas visuais</p>
            </div>
          )}

          {loading && (
            <div className="h-full min-h-[400px] flex flex-col items-center justify-center space-y-4">
              <div className="w-12 h-12 border-4 border-blue-500/20 border-t-blue-500 rounded-full animate-spin" />
              <p className="text-zinc-400 animate-pulse font-medium">Analisando algoritmos...</p>
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

export default App;