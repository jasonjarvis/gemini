using Gemini.Modules.UndoRedo;

namespace Gemini.Framework
{
	public interface IDocument : ILayoutItem
	{
        string DocumentPath { get; }
        IUndoRedoManager UndoRedoManager { get; }
	}
}