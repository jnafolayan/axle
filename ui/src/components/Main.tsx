export type TMainProps = {
  children: React.ReactNode;
};

export default function Main(props: TMainProps) {
  return (
    <div className="mt-2 pb-8 text-base md:text-lg text-gray-100">
      {props.children}
    </div>
  );
}
