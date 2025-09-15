import {useAtom} from "jotai";
import {AllGenresAtom} from "../atoms/atoms.ts";

export default function Genres() {
    
    const [genres] = useAtom(AllGenresAtom);
    
    return <>{
    genres.map(g => {
        return <div>{JSON.stringify(g)}</div>
    })
    }</>
}