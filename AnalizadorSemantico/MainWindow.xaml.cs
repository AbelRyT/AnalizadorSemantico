using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnalizadorSemantico
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> LexicalTokens { get; set; } = new();
        public ObservableCollection<string> SyntaxErrors { get; set; } = new();
        public ObservableCollection<string> SemanticErrors { get; set; } = new();

        public MainWindow()
        {
            this.InitializeComponent();
            LexicalListView.ItemsSource = LexicalTokens;
            SyntaxListView.ItemsSource = SyntaxErrors;
            SemanticListView.ItemsSource = SemanticErrors;
        }

        private void AnalyzeCode_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeInput.Text;
            AnalyzeCode(code);
        }

        private void AnalyzeCode(string code)
        {
            LexicalTokens.Clear();
            SyntaxErrors.Clear();
            SemanticErrors.Clear();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            // Análisis Léxico
            foreach (var token in root.DescendantTokens())
            {
                LexicalTokens.Add($"Token -> '{token.Text}'");
            }

            // Análisis Sintáctico
            foreach (var diagnostic in syntaxTree.GetDiagnostics())
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    SyntaxErrors.Add(diagnostic.GetMessage());
                }
            }

            // Análisis Semántico
            var compilation = CSharpCompilation.Create("CodeAnalysis")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(syntaxTree);

            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            foreach (var diagnostic in compilation.GetDiagnostics())
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    SemanticErrors.Add(diagnostic.GetMessage());
                }
            }
        }
    }
}