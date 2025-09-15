import {useAtom} from "jotai";
import {AllBooksAtom} from "../atoms/atoms.ts";

export default function Books() {
    
    const [books] = useAtom(AllBooksAtom);
    
    return <>
        {
            books.map(b => {
                return <div>{JSON.stringify(b)}</div>
            })
        }
    </>
}
