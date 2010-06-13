﻿#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Generic;
using System.Xml;
using BlogML;
using BlogML.Xml;

namespace Subtext.ImportExport
{
    public class BlogMLWriter : BlogMLWriterBase, IBlogMLWriter
    {
        public BlogMLWriter(IBlogMLSource source, bool embedAttachments)
        {
            EmbedAttachments = embedAttachments;
            Source = source;
        }

        public IBlogMLSource Source
        {
            get;
            private set;
        }

        public bool EmbedAttachments
        {
            get;
            set;
        }

        protected override void InternalWriteBlog()
        {
            var blog = Source.GetBlog();
            WriteStartBlog(blog.Title, ContentTypes.Text, blog.SubTitle, ContentTypes.Text, blog.RootUrl, blog.DateCreated);

            WriteAuthors(blog.Authors);
            WriteExtendedProperties(blog.ExtendedProperties);
            WriteCategories(blog.Categories);
            WritePosts(Source.GetBlogPosts(EmbedAttachments));

            WriteEndElement();
            Writer.Flush();
        }

        void IBlogMLWriter.Write(XmlWriter writer)
        {
            Write(writer);
        }

        private void WriteAuthors(IEnumerable<BlogMLAuthor> authors)
        {
            WriteStartAuthors();
            foreach(BlogMLAuthor bmlAuthor in authors)
            {
                WriteAuthor(
                    bmlAuthor.ID,
                    bmlAuthor.Title,
                    bmlAuthor.Email,
                    bmlAuthor.DateCreated,
                    bmlAuthor.DateModified,
                    bmlAuthor.Approved);
            }
            WriteEndElement(); // </authors>
        }

        private void WriteExtendedProperties(ICollection<Pair<string, string>> extendedProperties)
        {
            if(extendedProperties.Count > 0)
            {
                WriteStartExtendedProperties();
                foreach(var extProp in extendedProperties)
                {
                    WriteExtendedProperty(extProp.Key, extProp.Value);
                }
                WriteEndElement();
            }
        }

        private void WriteCategories(IEnumerable<BlogMLCategory> categories)
        {
            WriteStartCategories();
            foreach(BlogMLCategory category in categories)
            {
                WriteCategory(category.ID, category.Title, ContentTypes.Text, category.DateCreated, category.DateModified, category.Approved, category.Description, category.ParentRef);
            }
            WriteEndElement();
        }

        private void WritePosts(IEnumerable<BlogMLPost> posts)
        {
            WriteStartPosts();
            foreach(var post in posts)
            {
                WriteStartBlogMLPost(post);
                WritePostCategories(post.Categories);
                WritePostComments(post.Comments);
                WritePostTrackbacks(post.Trackbacks);
                WritePostAttachments(post.Attachments);
                WritePostAuthors(post.Authors);

                WriteEndElement(); // </post>
                Writer.Flush();
            }
            WriteEndElement();
        }

        protected void WriteStartBlogMLPost(BlogMLPost post)
        {
            WriteStartElement("post");
            WriteNodeAttributes(post.ID, post.DateCreated, post.DateModified, post.Approved);
            WriteAttributeString("post-url", post.PostUrl);
            WriteAttributeStringRequired("type", "normal");
            WriteAttributeStringRequired("hasexcerpt", post.HasExcerpt.ToString().ToLowerInvariant());
            WriteAttributeStringRequired("views", post.Views.ToString());
            WriteContent("title", BlogMLContent.Create(post.Title, ContentTypes.Text));
            WriteBlogMLContent("content", post.Content);
            if(!String.IsNullOrEmpty(post.PostName))
            {
                WriteContent("post-name", BlogMLContent.Create(post.PostName, ContentTypes.Text));
            }
            if(post.HasExcerpt)
            {
                WriteBlogMLContent("excerpt", post.Excerpt);
            }
        }

        protected void WriteBlogMLContent(string elementName, BlogMLContent content)
        {
            WriteContent(elementName, content);
        }

        protected void WritePostCategories(BlogMLPost.CategoryReferenceCollection categoryRefs)
        {
            if(categoryRefs.Count > 0)
            {
                WriteStartCategories();
                foreach(BlogMLCategoryReference categoryRef in categoryRefs)
                {
                    WriteCategoryReference(categoryRef.Ref);
                }
                WriteEndElement();
            }
        }

        private void WritePostComments(BlogMLPost.CommentCollection comments)
        {
            if(comments.Count > 0)
            {
                WriteStartComments();
                foreach(BlogMLComment comment in comments)
                {
                    string userName = string.IsNullOrEmpty(comment.UserName) ? "Anonymous" : comment.UserName;
                    WriteComment(comment.ID, BlogMLContent.Create(comment.Title, ContentTypes.Text), comment.DateCreated, comment.DateModified,
                                 comment.Approved, userName, comment.UserEMail, comment.UserUrl,
                                 comment.Content);
                }
                WriteEndElement();
            }
        }

        private void WritePostTrackbacks(BlogMLPost.TrackbackCollection trackbacks)
        {
            if(trackbacks.Count > 0)
            {
                WriteStartTrackbacks();
                foreach(BlogMLTrackback trackback in trackbacks)
                {
                    if(!String.IsNullOrEmpty(trackback.Url))
                    {
                        WriteTrackback(trackback.ID, trackback.Title, ContentTypes.Text, trackback.DateCreated, trackback.DateModified, trackback.Approved, trackback.Url);
                    }
                }
                WriteEndElement();
            }
        }

        private void WritePostAttachments(BlogMLPost.AttachmentCollection attachments)
        {
            if(attachments.Count > 0)
            {
                WriteStartAttachments();
                foreach(BlogMLAttachment attachment in attachments)
                {
                    if(attachment.Embedded)
                    {
                        WriteAttachment(attachment.Url, attachment.Data.Length, attachment.MimeType, attachment.Path, attachment.Embedded, attachment.Data);
                    }
                    else
                    {
                        WriteAttachment(attachment.Path, attachment.MimeType, attachment.Url);
                    }
                }
                WriteEndElement(); // End Attachments Element
                Writer.Flush();
            }
        }

        private void WritePostAuthors(BlogMLPost.AuthorReferenceCollection authorsRefs)
        {
            if(authorsRefs.Count > 0)
            {
                WriteStartAuthors();
                foreach(BlogMLAuthorReference authorRef in authorsRefs)
                {
                    WriteAuthorReference(authorRef.Ref);
                }
                WriteEndElement();
            }
        }
    }
}
