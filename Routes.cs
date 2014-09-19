using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace GoodM8s.Basketball {
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes()) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 9,
                    Route = new Route(
                        "Basketball",
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"},
                            {"controller", "Home"},
                            {"action", "Summary"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 9,
                    Route = new Route(
                        "Fixtures/{sportId}/{round}",
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"},
                            {"controller", "Home"},
                            {"action", "Fixtures"},
                            {"sportId", 0},
                            {"round", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 9,
                    Route = new Route(
                        "Ladder/{sportId}/{round}",
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"},
                            {"controller", "Home"},
                            {"action", "Ladder"},
                            {"sportId", 0},
                            {"round", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 9,
                    Route = new Route(
                        "Statistics/{sportId}/{gameId}",
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"},
                            {"controller", "Home"},
                            {"action", "Statistics"},
                            {"sportId", 0},
                            {"gameId", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 9,
                    Route = new Route(
                        "Vs/{sportId}/{round}",
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"},
                            {"controller", "Home"},
                            {"action", "Vs"},
                            {"sportId", 0},
                            {"round", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "GoodM8s.Basketball"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}