import clsx from "clsx";
import React, { useState } from "react";
import Container from "../components/Container";
import Header from "../components/Header";
import Main from "../components/Main";
import api from "../helpers/api";

export default function Admin() {
  const [currentTab, setCurrentTab] = useState<TTabs>("document");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState("");

  const gotoTab = (event: React.MouseEvent<HTMLButtonElement>) => {
    const { tab } = event.currentTarget.dataset;
    setCurrentTab(tab! as TTabs);
  };

  const handleSubmit = (payload: TSubmitPayload) => {
    setMessage("");
    setError(null);
    api.documents
      .upload(payload)
      .then(() => {
        setMessage("Resource was uploaded successfully");
        setTimeout(() => setMessage(""), 5000);
      })
      .catch(() => {
        setError("Could not upload the resource(s)");
        setTimeout(() => setError(null), 5000);
      });
  };

  const handleAbsolutePathSubmit = (url: string) => {
    setMessage("");
    setError(null);
    api.documents
      .absolutePath(url)
      .then(() => {
        setMessage("Resource was uploaded successfully");
      })
      .catch(() => {
        setError("Could not upload the resource(s)");
      });
  };

  return (
    <Container>
      <Header />

      <div className="my-4 h-1 bg-gray-800"></div>

      <Main>
        <h1 className="text-md md:text-xl mb-4 text-gray-400">
          Administration
        </h1>
        <div className="space-y-6">
          <div>
            <h3 className="text-base md:text-xl mb-2 uppercase">
              Upload Resources
            </h3>
            <div className="px-4 border-l-4 border-primary-400">
              {message ? (
                <p className="text-base text-green-500 mb-3">{message}</p>
              ) : null}
              {error ? (
                <p className="text-base text-red-500 mb-3">{error}</p>
              ) : null}
              {/* tabs */}
              <div>
                <Tab
                  label="Document"
                  tab="document"
                  currentTab={currentTab}
                  onClick={gotoTab}
                />
                <Tab
                  label="URL"
                  tab="url"
                  currentTab={currentTab}
                  onClick={gotoTab}
                />
                <Tab
                  label="Sitemap"
                  tab="sitemap"
                  currentTab={currentTab}
                  onClick={gotoTab}
                />
                <Tab
                  label="Absolute Path"
                  tab="absolute"
                  currentTab={currentTab}
                  onClick={gotoTab}
                />
              </div>
              {/* tab content */}
              <div className="py-4">
                <form action="" method="POST" autoComplete="off">
                  {currentTab === "document" ? (
                    <DocumentUpload onSubmit={handleSubmit} />
                  ) : currentTab === "url" ? (
                    <URLUpload onSubmit={handleSubmit} />
                  ) : currentTab === "sitemap" ? (
                    <SitemapUpload onSubmit={handleSubmit} />
                  ) : (
                    <AbsolutePathUpload onSubmit={handleAbsolutePathSubmit} />
                  )}
                </form>
              </div>
            </div>
          </div>
        </div>
      </Main>
    </Container>
  );
}

type TTabs = "document" | "url" | "sitemap" | "absolute";
type TTabProps = {
  label: string;
  tab: TTabs;
  currentTab: TTabs;
  onClick: (event: React.MouseEvent<HTMLButtonElement>) => void;
};
function Tab({ label, tab, currentTab, onClick }: TTabProps) {
  return (
    <button
      className={clsx(
        "w-36 text-center py-2 px-3 border-b hover:bg-primary-500 hover:text-gray-900",
        {
          "bg-primary-500 text-gray-800": currentTab === tab,
        }
      )}
      data-tab={tab}
      onClick={onClick}
    >
      {label}
    </button>
  );
}

type TSubmitPayload = {
  type: string;
  title?: string;
  description?: string;
  documents?: File[];
  link?: string;
};

type TUploadProps = {
  onSubmit: (payload: TSubmitPayload) => void;
};

function DocumentUpload(props: TUploadProps) {
  const [files, setFiles] = useState<File[]>([]);
  const [title, setTitle] = useState("");

  const handleFiles = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event.target.files;
    if (files && files.length) {
      setFiles(Array.from(files));
    } else {
      // setFiles([]);
    }
  };

  const handleTitleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setTitle(event.target.value);
  };

  const removeFileAt = (index: number) => {
    setFiles(files.filter((_, i) => i !== index));
  };

  const submit = () => {
    if (!files.length) return;
    props.onSubmit({
      type: "document",
      title,
      documents: files,
    });
  };

  return (
    <div className="space-y-4">
      <div>
        <label htmlFor="title" className="block text-base">
          Title
        </label>
        <input
          id="title"
          type="text"
          placeholder="Enter title"
          onChange={handleTitleChange}
          className="w-full block max-w-lg border bg-transparent rounded-lg text-lg outline-none px-3 py-2"
        />
      </div>

      <label htmlFor="documents" className="block">
        <div className="w-full h-24 border border-dashed rounded-lg flex items-center justify-center cursor-pointer">
          <input
            id="documents"
            type="file"
            multiple
            hidden
            onChange={handleFiles}
          />
          Click here to upload files
        </div>
      </label>

      {files.length ? (
        <div>
          {files.map((file, idx) => (
            <div
              key={idx}
              className="max-w-lg text-base flex justify-between items-center"
            >
              <p key={idx} className="truncate">
                {file.name}
              </p>
              <button
                type="button"
                className="text-red-500"
                onClick={() => removeFileAt(idx)}
              >
                Remove
              </button>
            </div>
          ))}
        </div>
      ) : null}

      <div>
        <button
          type="button"
          className="py-1 px-3 w-24 block ml-auto bg-gray-100 text-gray-900"
          onClick={submit}
        >
          Submit
        </button>
      </div>
    </div>
  );
}

function URLUpload(props: TUploadProps) {
  const [url, setURL] = useState<string>("");

  const handleURLChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const url = event.target.value;
    setURL(url);
  };

  const submit = () => {
    if (!url) return;
    props.onSubmit({
      type: "url",
      link: url,
    });
  };

  return (
    <div>
      <label htmlFor="url" className="block text-base mb-2">
        URL
      </label>
      <input
        id="url"
        type="text"
        placeholder="Enter URL"
        onChange={handleURLChange}
        className="w-full max-w-lg border bg-transparent rounded-lg text-lg outline-none px-3 py-2"
      />
      <div>
        <button
          type="button"
          className="py-1 px-3 w-24 block ml-auto bg-gray-100 text-gray-900"
          onClick={submit}
        >
          Submit
        </button>
      </div>
    </div>
  );
}

function SitemapUpload(props: TUploadProps) {
  const [file, setFile] = useState<File | null>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event.target.files;
    if (files && files.length) {
      setFile(files[0]);
    } else {
      // setFiles([]);
    }
  };

  const submit = () => {
    if (!file) return;
    props.onSubmit({
      type: "sitemap",
      documents: [file],
    });
  };

  return (
    <div className="space-y-4">
      <label htmlFor="documents">
        <div className="w-full h-24 border border-dashed rounded-lg flex items-center justify-center cursor-pointer">
          <input
            id="documents"
            type="file"
            multiple
            hidden
            onChange={handleFileChange}
          />
          Click here to upload sitemap file
        </div>
      </label>

      <div>
        <button
          type="button"
          className="py-1 px-3 w-24 block ml-auto bg-gray-100 text-gray-900"
          onClick={submit}
        >
          Submit
        </button>
      </div>
    </div>
  );
}

type TAbsolutePathUploadProps = {
  onSubmit: (url: string) => void;
};

function AbsolutePathUpload(props: TAbsolutePathUploadProps) {
  const [url, setURL] = useState<string>("");

  const handleURLChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const url = event.target.value;
    setURL(url);
  };

  const submit = () => {
    if (!url) return;
    props.onSubmit(url);
  };

  return (
    <div>
      <label htmlFor="url" className="block text-base mb-2">
        Absolute Path URL
      </label>
      <input
        id="url"
        type="text"
        placeholder="Enter URL"
        onChange={handleURLChange}
        className="w-full max-w-lg border bg-transparent rounded-lg text-lg outline-none px-3 py-2"
      />
      <div>
        <button
          type="button"
          className="py-1 px-3 w-24 block ml-auto bg-gray-100 text-gray-900"
          onClick={submit}
        >
          Submit
        </button>
      </div>
    </div>
  );
}
