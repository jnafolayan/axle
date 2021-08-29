import { Link } from "react-router-dom";
import { TSearchResultDocument } from "../helpers/api";

export type TSearchDocumentProps = {
  value: TSearchResultDocument;
};

export default function SearchDocument(props: TSearchDocumentProps) {
  const { value: doc } = props;
  return (
    <div className="w-full">
      <Link to={doc.link}>{doc.title}</Link>
      <p>{doc.description}</p>
    </div>
  );
}