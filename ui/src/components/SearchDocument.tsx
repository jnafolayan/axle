import api, { TSearchResultDocument } from "../helpers/api";

export type TSearchDocumentProps = {
  value: TSearchResultDocument;
};

export default function SearchDocument(props: TSearchDocumentProps) {
  const { value: doc } = props;
  const title = doc.title || "Untitled";
  const description = doc.description || "No description";
  return (
    <div className="w-full">
      <a
        href={api.documents.getDownloadURL(doc.link)}
        target="_blank"
        className="text-base block md:text-xl text-primary-500 hover:underline truncate"
      >
        {title}
      </a>
      <p className="text-sm lg:text-base text-gray-200">{description}</p>
    </div>
  );
}
