using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Constants
{
	/// <summary>
	/// Constants related to the documents handled in our site
	/// </summary>
	public sealed class Documents
	{
		/// <summary>
		/// Templates related to Documents handling
		/// </summary>
		public sealed class Templates
		{           

			/// <summary>
			/// Constants for the Document Base template
			/// (Item: /sitecore/templates/Genworth/Base/Document Base)
			/// </summary>
			public sealed class DocumentBase
			{
				/// <summary>
				/// Document Base template name 
				/// </summary>
				public const string Name = "Document Base";

				/// <summary>
				/// Sections associated to the document base template
				/// </summary>
				public sealed class Sections
				{
					/// <summary>
					/// Document Section associated to the Document Base template
					/// (Item: /sitecore/templates/Genworth/Base/Document Base/Document)
					/// </summary>
					public sealed class Document
					{
						/// <summary>
						/// Document Section Name
						/// </summary>
						public const string Name = "Document";

						/// <summary>
						/// Fields associated to the documnet section
						/// </summary>
						public sealed class Fields
						{
							/// <summary>
							/// File field 
							/// (Item: /sitecore/templates/Genworth/Base/Document Base/Document/File)
							/// (Type: File)
							/// </summary>
							public const string File = "File";


							/// <summary>
							/// File field 
							/// (Item: /sitecore/templates/Genworth/Base/Document Base/Document/Thumbnail)
							/// (Type: Image)
							/// </summary>
							public const string Thumbnail = "Thumbnail";

						}
					}
				}
			}
		}

		/// <summary>
		/// Constants related to the indexes used by Lucene to index document
		/// </summary>
		public sealed class Indexes
		{
			/// <summary>
			/// Constants for the index that handles the indexing of items that inherit from the Base Document Template
			/// </summary>
			public sealed class DocumnetBaseIndex
			{

				public const string Name = "DocumentsIndex";
				/// <summary>
				/// Constants for the fields handled by the index
				/// </summary>
				public sealed class Fields
				{

					/// <summary>
					/// File Extension					
					/// </summary>
					public const string Extension = "extension";

					/// <summary>
					/// File MIME Type	
					/// </summary>
					public const string MIME = "mime";
					/// <summary>
					/// Content
					/// </summary>
					public const string CONTENT = "contentfile";

                    /// <summary>
                    /// Path
                    /// </summary>
                    public const string PATH = "path";
				}
			}
		}
	}
}
