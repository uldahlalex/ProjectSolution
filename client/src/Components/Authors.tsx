import {useAtom} from "jotai";
import {AllAuthorsAtom, AllBooksAtom} from "../atoms/atoms.ts";
import {useEffect, useState} from "react";
import {type Author, type CreateAuthorRequestDto, LibraryClient} from "../generated-client.ts";
import {AuthorDetails} from "./AuthorDetails.tsx";
import useLibraryCrud, {libraryApi} from "../useLibraryCrud.ts";
import {resolveRefs} from "dotnet-json-refs";

export default function Authors() {

    //const [authors, setAllAuthors] = useAtom(AllAuthorsAtom);
    const [authors, setAuthors] = useState<Author[]>([])
    const [books] = useAtom(AllBooksAtom);
    const [createAuthorForm, setCreateAuthorForm] = useState<CreateAuthorRequestDto>({
        name: "New Author"
    });
    const libraryCrud = useLibraryCrud();
    
    useEffect(() => {
        libraryApi.getAuthors().then(result => {
            const circularRefsHandled = resolveRefs(result);
            setAuthors(circularRefsHandled)
        })
    }, [])


    return <>
        <div className="p-5">
            <h2 className="text-2xl font-bold text-primary mb-4">✍️ Authors</h2>

            <div className="card bg-base-100 shadow-lg border border-base-300 mb-6">
                <div className="card-body p-6">
                    <h3 className="card-title text-lg mb-4 flex items-center gap-2">
                        <span>➕</span> Create New Author
                    </h3>
                    <div className="flex gap-4 items-end">
                        <div className="form-control flex-1">
                            <label className="label">
                                <span className="label-text font-medium">Author Name</span>
                            </label>
                            <input
                                value={createAuthorForm.name}
                                placeholder="Enter author name"
                                className="input input-bordered w-full"
                                onChange={e => setCreateAuthorForm({...createAuthorForm, name: e.target.value})}
                            />
                        </div>
                        <button
                            className="btn btn-primary gap-2"
                            onClick={() => {
                                libraryCrud.createAuthor(createAuthorForm);
                                setCreateAuthorForm({name: "New Author"});
                            }}
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="w-4 h-4 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4"></path>
                            </svg>
                            Create Author
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <ul className="list bg-base-100 rounded-box shadow-md mx-5">
            {
                authors.map(a => <AuthorDetails key={a.id} author={a} />)
            }
        </ul>
    </>
}