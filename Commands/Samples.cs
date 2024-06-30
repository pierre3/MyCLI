partial class MyCommands
{
    public void Search(string category, string sort, string filter)
    {
        Console.WriteLine($"Running... search --category {category} --sort {sort} --filter {filter}");
    }

    public void Share(string platform, string visibility, string tag)
    {
        Console.WriteLine($"Running... share --platform {platform} --visibility {visibility} --tag {tag}");
    }

    public void Edit(string file, string mode, bool backup)
    {
        Console.WriteLine($"Running... edit --file {file} --mode {mode} --backup {backup}");
    }

    public void View(string layout, string sort, string filter)
    {
        Console.WriteLine($"Running... view --layout {layout} --sort {sort} --filter {filter}");
    }
}