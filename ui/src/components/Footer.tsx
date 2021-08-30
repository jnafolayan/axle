import { Link } from "react-router-dom";

export default function Footer() {
  return (
    <footer className="w-full">
      <p className="text-gray-600 text-center">
        View on{" "}
        <a
          href="https://github.com/jnafolayan/axle"
          target="_blank"
          className="text-primary-400"
        >
          Github
        </a>
      </p>
    </footer>
  );
}
