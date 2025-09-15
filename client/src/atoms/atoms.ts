import {atom} from "jotai/vanilla/atom";
import type {AuthorDto, BookDto, GenreDto} from "../generated-client.ts";

export const AllAuthorsAtom = atom<AuthorDto[]>([]);
export const AllBooksAtom = atom<BookDto[]>([]);
export const AllGenresAtom = atom<GenreDto[]>([]);