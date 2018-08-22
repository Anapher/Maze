using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileExplorer.Administration.ViewModels
{
	public class NavigationBarViewModel : BindableBase
	{
	    private readonly FileExplorerViewModel _fileExplorerViewModel;

	    public NavigationBarViewModel(FileExplorerViewModel fileExplorerViewModel)
	    {
	        _fileExplorerViewModel = fileExplorerViewModel;
	    }

	    private DelegateCommand _goBackCommand;

	    public DelegateCommand GoBackCommand
	    {
	        get
	        {
	            return _goBackCommand ?? (_goBackCommand = new DelegateCommand(() =>
	            {
	                
	            }));
	        }
	    }

	    private DelegateCommand _goForwardCommand;

	    public DelegateCommand GoForwardCommand
	    {
	        get
	        {
	            return _goForwardCommand ?? (_goForwardCommand = new DelegateCommand(() =>
	            {
	                
	            }));
	        }
	    }
	}
}
