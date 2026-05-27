export interface SearchResult {
    algorithmName: string;
    theoreticalComplexity: string;
    shortComplexity: string;
    positions: number[];
    comparisons: number;
    spuriousHits: number;
    elapsedMilliseconds: number;
    elapsedNanoseconds: number;
    steps: string[];
    textLength: number;
    patternLength: number;
    auxiliaryData: Record<string, string>;
}

export interface AlgorithmInfo {
    algorithmName: string;
    theoreticalComplexity: string;
    shortComplexity: string;
    bestUseCase: string;
}
