import type {SieveModel} from "ts-sieve-query-builder";

export default function parameterizeSieveModel(sieveModel: SieveModel) {
    return [sieveModel.filters, sieveModel.sorts, sieveModel.page, sieveModel.pageSize] as const;
}