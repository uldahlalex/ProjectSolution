import {useAtom} from "jotai";
import {AllAuthorsAtom} from "../atoms/atoms.ts";

export default function Authors() {
    
    const [authors] = useAtom(AllAuthorsAtom)
    
    return <>{
    authors.map(a => {
        return <div>{JSON.stringify(a)}</div>
    })
    }</>
}