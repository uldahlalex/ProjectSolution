import {useLocation, useNavigate, useSearchParams} from "react-router";
import {SieveQueryBuilder} from "ts-sieve-query-builder";
import type {Author, Book} from "../generated-client.ts";
import {useEffect, useState} from "react";
import {libraryApi} from "../useLibraryCrud.ts";
import parameterizeSieveModel from "../parameterizeSieveModel.ts";
import {resolveRefs} from "dotnet-json-refs";

export default function SieveFiltering() {

    const location = useLocation();
    const navigate = useNavigate();
    const searchString = location.search;
    console.log("useLocation search string: "+searchString)
    const sieveQuery = SieveQueryBuilder.parseQueryString<Book>(searchString)
    console.log("sieve query object: "+JSON.stringify(sieveQuery))
    console.log("sieve model: "+JSON.stringify(sieveQuery.buildSieveModel()));
    console.log("sieve query string: "+JSON.stringify(sieveQuery.buildQueryString()))
    console.log("sieve query params: "+JSON.stringify(sieveQuery.buildQueryParams()))
    useEffect(() => {
    const params = parameterizeSieveModel(sieveQuery.buildSieveModel());
        libraryApi.getBooks(...params).then(r => {
            console.log(resolveRefs(r))
        })
    }, [location])
    return <>
    
    <input placeholder="title"  onChange={e => {
        let q = sieveQuery;
        // Remove existing title filter(s) first
        q = q.removeFilters("title"); // if this method exists
        // Then add the new one
        q = q.filterContains("title", e.target.value);        console.log("query params: "+JSON.stringify(q.buildQueryParams()))
        console.log("query string: "+JSON.stringify(q.buildQueryString()))
        navigate({
            pathname: '.',
            search: ""+q.buildQueryString(),
            hash: ''
        })
    }} />
    </>
    
}