﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Medical.Irony
{
    [Language("XML", "1", "XML Grammar")]
    public class XmlGrammar : Grammar
    {
        public XmlGrammar()
            : base(false)
        {
            //Terminals
            Terminal comment = new CommentTerminal("comment", "<!--", "-->");
            NonGrammarTerminals.Add(comment);
            var number = new NumberLiteral("number");
            var stringLiteral = new StringLiteral("string", "\"", StringOptions.None);
            var stringContent = new XmlContentText("StringContent");
            KeyTerm elementOpener = ToTerm("<");
            KeyTerm closeElementOpener = ToTerm("</");
            KeyTerm elementCloser = ToTerm(">");
            KeyTerm openCloseElementCloser = ToTerm("/>");
            KeyTerm equals = ToTerm("=");
            KeyTerm xmlDeclOpen = ToTerm("<?");
            KeyTerm xmlDeclClose = ToTerm("?>");

            IdentifierTerminal identifier = new IdentifierTerminal("Identifier");

            //Non Terminals
            NonTerminal document = new NonTerminal("document");
            NonTerminal elementStart = new NonTerminal("ElementStart");
            NonTerminal elementEnd = new NonTerminal("ElementEnd");
            NonTerminal openCloseElement = new NonTerminal("OpenCloseElement");
            NonTerminal element = new NonTerminal("Element");
            NonTerminal requiredElements = new NonTerminal("RequiredElements");
            NonTerminal innerContent = new NonTerminal("InnerContent");
            NonTerminal content = new NonTerminal("Content");
            NonTerminal attribute = new NonTerminal("Attribute");
            NonTerminal optionalAttribute = new NonTerminal("OptionalAttribute");
            NonTerminal xmlDeclaration = new NonTerminal("XmlDeclaration");
            NonTerminal optionalXmlDeclaration = new NonTerminal("OptionalXmlDeclaration");

            //Rules
            this.Root = document;

            innerContent.Rule = element | stringContent;
            content.Rule = MakeStarRule(content, innerContent);

            attribute.Rule = identifier + equals + stringLiteral;
            optionalAttribute.Rule = MakeStarRule(optionalAttribute, attribute);

            elementStart.Rule = elementOpener + identifier + optionalAttribute + elementCloser;
            elementEnd.Rule = closeElementOpener + identifier + elementCloser;
            openCloseElement.Rule = elementOpener + identifier + optionalAttribute + openCloseElementCloser;

            element.Rule = (elementStart + content + elementEnd) | openCloseElement;
            requiredElements.Rule = MakePlusRule(requiredElements, element);

            xmlDeclaration.Rule = xmlDeclOpen + identifier + optionalAttribute + xmlDeclClose;
            optionalXmlDeclaration.Rule = MakeStarRule(optionalXmlDeclaration, xmlDeclaration);

            document.Rule = optionalXmlDeclaration + requiredElements;

            MarkPunctuation(elementOpener, elementCloser, closeElementOpener, openCloseElementCloser, equals, xmlDeclOpen, xmlDeclClose);
            MarkTransient(innerContent);
        }

    }

    public class XmlContentText : Terminal
    {
        public XmlContentText(String name)
            : base(name)
        {

        }

        public override IList<string> GetFirsts()
        {
            return null;
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            int stopIndex = source.Text.IndexOf('<', source.Location.Position);
            if (stopIndex == source.Location.Position)
            {
                return null;
            }
            if (stopIndex < 0)
            {
                stopIndex = source.Text.Length;
            }
            source.PreviewPosition = stopIndex;
            return source.CreateToken(this.OutputTerminal);
        }
    }
}