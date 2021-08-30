type TContainerProps = {
  children: React.ReactNode;
};

export default function Container(props: TContainerProps) {
  return <div className="max-w-5xl mx-auto py-3 px-4">{props.children}</div>;
}
