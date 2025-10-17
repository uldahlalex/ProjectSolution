import {useSearchParams} from "react-router";
import {SieveQueryBuilder} from "ts-sieve-query-builder";
import type {Author, Book} from "../generated-client.ts";
import {useState} from "react";
import {libraryApi} from "../useLibraryCrud.ts";
import parameterizeSieveModel from "../parameterizeSieveModel.ts";
import {resolveRefs} from "dotnet-json-refs";

export default function SieveFiltering() {
    
    const [searchParams] = useSearchParams();
    const [form, setForm] = useState<Book>({
        title: "",
        id: "",
        genre: undefined,
        authors: [],
        createdat: new Date().toUTCString(),
        genreid: undefined,
        pages: 42
    })
    const sieveQuery = SieveQueryBuilder.create<Book>()
            .filterContains("title", form.title)
            .filterGreaterThan("createdat", form.createdat);
    
    return <>
    
    <input placeholder="title" defaultValue={form.title} onChange={e => {
        console.log(form.createdat)
        console.log(e.target.value)
        const d = new Date(e.target.value)
        console.log(d)
        const a = d.toISOString()
        console.log(a)
        console.log(d.toUTCString())
        setForm({...form, title: d.toUTCString()});
    }} />
    <input placeholder="date" type="date" defaultValue={form.createdat} onChange={e => setForm({...form,  title: e.target.value})} />
    
        <button onClick={() =>{
            libraryApi.getBooks(...parameterizeSieveModel(sieveQuery.buildSieveModel())).then(r => {
                console.log(resolveRefs(r))
            })
        }}>Submit</button>
        
    </>
    
}