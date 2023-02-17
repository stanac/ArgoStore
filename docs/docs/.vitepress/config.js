export default {
  lang: 'en-US',
  title: 'ArgoStore',
  description: '.NET Embedded Transactional Document DB',
  cleanUrls: true,
  
  ignoreDeadLinks: true,

  themeConfig: {
    repo: 'stanac/ArgoStore',
    docsDir: 'docs',
    docsBranch: 'rework',

    socialLinks: [
      { 
        icon: 'github',
        link: 'https://github.com/stanac/ArgoStore' 
      },
      {
        icon: {
          svg: '<svg width="24px" height="24px" viewBox="0 0 512 512" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"><title>nuget</title><defs><polygon id="path-1" points="0 46.021103 0 3.7002935 84.6521577 3.7002935 84.6521577 88.3419125 0 88.3419125"/></defs><g id="Symbols" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"><g id="nuget"><g id="Global/Logo" transform="translate(0.000000, 6.000000)"><path d="M374.424959,454.856991 C327.675805,454.856991 289.772801,416.950177 289.772801,370.196324 C289.772801,323.463635 327.675805,285.535656 374.424959,285.535656 C421.174113,285.535656 459.077116,323.463635 459.077116,370.196324 C459.077116,416.950177 421.174113,454.856991 374.424959,454.856991 M205.565067,260.814741 C176.33891,260.814741 152.657469,237.109754 152.657469,207.901824 C152.657469,178.672728 176.33891,154.988907 205.565067,154.988907 C234.791225,154.988907 258.472666,178.672728 258.472666,207.901824 C258.472666,237.109754 234.791225,260.814741 205.565067,260.814741 M378.170817,95.6417786 L236.886365,95.6417786 C164.889705,95.6417786 106.479717,154.057639 106.479717,226.082702 L106.479717,367.360191 C106.479717,439.40642 164.889705,497.77995 236.886365,497.77995 L378.170817,497.77995 C450.209803,497.77995 508.577466,439.40642 508.577466,367.360191 L508.577466,226.082702 C508.577466,154.057639 450.209803,95.6417786 378.170817,95.6417786" id="Fill-12" fill="#666" fill-rule="evenodd"/><mask id="mask-2" fill="white"><use xlink:href="#path-1"/></mask><g id="Clip-15"/><path d="M84.6521577,46.0115787 C84.6521577,69.3990881 65.6900744,88.3419125 42.3260788,88.3419125 C18.9409203,88.3419125 0,69.3990881 0,46.0115787 C0,22.6452344 18.9409203,3.68124485 42.3260788,3.68124485 C65.6900744,3.68124485 84.6521577,22.6452344 84.6521577,46.0115787" id="Fill-14" fill="#666" fill-rule="evenodd" mask="url(#mask-2)"/></g></g></g></svg>'
        },
        link: 'https://www.nuget.org/packages/ArgoStore'
      }
    ],

    nav: [
      { text: 'Get Started', link: '/docs/introduction/getting-started' },
      { text: 'ASP.NET Core Integration', link: '/docs/introduction/getting-started-aspnetcore' }
      
    ],

    sidebar: [
      {
        text: 'Introduction',
        items: [
          {
            text: 'Get Started',
            link: '/docs/introduction/getting-started'
          },
          {
            text: 'ASP.NET Core Integration',
            link: '/docs/introduction/getting-started-aspnetcore'
          }
        ]
      },
      {
        text: 'Configuration',
        items: [
          {
            text: 'Configuration',
            link: '/docs/configuration/configuration'
          },
          {
            text: 'Identity',
            link: '/docs/configuration/identity'
          },
          {
            text: 'Indexing',
            link: '/docs/configuration/indexing'
          },
          {
            text: 'Optimistic Concurrency',
            link: '/docs/configuration/optimistic-concurrency'
          },
          {
            text: 'Multitenancy',
            link: '/docs/configuration/multitenancy'
          },
          {
            text: 'Underlying Tables',
            link: '/docs/configuration/underlying-tables'
          },
          {
            text: 'Logging',
            link: '/docs/configuration/logging'
          }
        ]
      },{
        text: 'CRUD(U)',
        items: [
          {
            text: 'Create',
            link: '/docs/crudu/create'
          },
          {
            text: 'Read',
            link: '/docs/crudu/read'
          },
          {
            text: 'Update',
            link: '/docs/crudu/update'
          },
          {
            text: 'Delete',
            link: '/docs/crudu/delete'
          },
          {
            text: 'Upsert',
            link: '/docs/crudu/upsert'
          },
          {
            text: 'Async operations',
            link: '/docs/crudu/async-operations'
          }
        ]
      }
    ]
  }
}