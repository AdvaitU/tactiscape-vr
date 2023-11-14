# Personal Logs

### Description

## June
### June 15 - June 30    

#### First ideas - Really Thinking about what I want to build
When I first got off into the summer break, I had mainly one thing in mind. I knew I wanted to make something that critically examines the role of tactility in the tangibility of a system. I'd been interested in the subject for the past two years and I'd been reading up a lot of material over the third semester (Quick Shout out to Mark Weiser's original [Ubiquitous Computing essay](http://scihi.org/mark-weiser-ubiquituous-computing/), still one of my dearest!). And I also knew I wanted to make something really really cool. In my head, the perfect amalgamation of both these objectives was Iron Man. Robert Downey Jr. muttering to himself as he swishes his hands here and there in a room filled with holograms was ESSENTIALLY cool. And so I started from there.
![Robert Downey Jr in Iron Man building his suit](https://techthatthrills.files.wordpress.com/2020/08/iron-man.jpg)   

#### 3D Sculpting - Setting the scope
Knowing I wanted to create something of the sort, I watched Robert Downey Jr.'s Iron Man - the whole film, and then the suit building scene from Iron Man 2 again and again. It proved to be the perfect way to inspire myself while technically also counting as downtime having just finished the third semester of the MSc. And therein lay my first idea for the thesis - Iron Man was building suits, I could build anything. Having used CAD as an industrial designer for close to 5 years by this point and then new modelling environments such as Unreal and Blender while on the MSc., I was all too familiar with the complaints around it. Navigating a 3D environment in 2D is uneccesarily hard. Sure, I've gotten used to it now, over time, but I've never really enjoyed it. A few too many times in the past as I try to make an edit here or a delete there, I've gone wrong - not because I didn't know how to use the tools but because, I believe, the tools weren't really designed well.  "If I could model anything I wanted just like Tony Stark does with his hands, the whole process would be so much easier. Because it would be a lot more tangible! And more fun too! "     

#### What does it mean for something to be tangible? And tactile?
Before the school was officially on break for the summer, I got an opportunity to sepak with my supervisor Phoenix and the first thing she asked me to do was define what 'tangibility' and 'tactility'. These were terms I'd been throwing around but never really critically examined. I sat down and came up with these definitions - an extension of dictionary defintions based on individual experience:

While traditionally, tangibility is related intrinsically to touch, its definition can be extended to include other sense that one is capable of perceiving. If something is tangible, that means it is comprehensible through some means. Hence, tangibility could be defined as:     
    
**Tangibility**: The perceptibility of an object.    

Tactility, on the other hand, is more grounded in touch. But when Iron Man uses holograms to create his suit, he is not touching any tactile thing at all. He is performing the actions that he would with an otherwise tactile form with with results that match expectations of tactile objects. So tactility can be broken down into two definitions:              
    
**a) Tactility**: The capability of an object to be touched.     
     
But also for my context:      
**b) Tactility**: The capability of an object to behave as though being touched - whether through physical touch or through simulated touch like haptic feedback, etc.
     
## July
### July 01 - July 15 
#### Why 3D Sculpting though??
Of course making a whole tactile computer was too large of a scope for the project so I decided to sit down and think about what computing processes could best be enhanced from the tactility I was imagining. Having spent years as an Industrial Designer and consequently a CAD Modeller, 3D Modelling was an obvious choice. 3D Modelling is difficult, it is not easy to visualise 3 dimensional objects on a computer screen and manipulate them effectively. Morover, having used Blender and Unreal Engine during my MSc., I had found myself even more frustrated by 3D Sculpting - the process that was meant to replicate real world affordances and make modelling easier. After some critical thought, I came to the conclusion that 3D Sculpting as an intuitive process is a promise unkept by its restriction to the same means of interaction as 3D modelling. While the process is inherently easier to understand, it still suffers from the same cognitive load drawn by using 2D means of representation such as a display and 2D means of interaction such as a computer mouse. It is for this reason that I decided to work on 3D sculpting - to enhance it and complete its promise of a fundamentally intuitive way of CAD.    

#### Has anything like this been done before? - Starting the literature review
The next step was to recognise the efforts that have gone into the field so far. I started by tracing my steps back to the original pieces such as Mark Weiser and Ishii and Ulmer's work that inspired me and did a deep dive into the history of tactility in computing. Following this, I started looking for material on tactility in 3D Modelling. To my astonishment, I discovered upwards of 30 pieces of literature and projects where people had worked towards the same goal, albeit in their own varying ways. I spent most of the fortnight simply collecting documents and reading abstracts to gain a fundamental understanding of the space I was trying to innovate within.     

#### Free Form Deformation - How could I actually make something like this?
While looking up the various approaches people have taken to creating such a system, I came across Free Form Deformation (FFD for short). FFD is a technique that has been around since at least as far back as 1986. It involves the creation of a hull object around a 3D model much like 2D vector graphics that can be pulled, pushed and rotated in different angles to deform the enclosed 3D Model. While this is not the most intuitive means of modelling, to me it represented an approach that would prove to be the basis of the tool I created for the thesis - the idea that I can move singular points in space in an intuitive manner replicating the kind of interaction used for physical clay modelling in order to manipulate entire objects.    

#### I need to turn the scope waaaaaaay down - Landscape Sculpting
The discovery if FFD excited me into my first foray into actually writing code for the project. I decided to download versions of FFD implementation and run them in a variety of environments such as OpenFrameworks, Processing, and WebGL. Although I was only slightly successful in this quest, I realised one thing really quickly - 3D forms are very very DIFFICULT. Keeping track of a vast array of 3D vertices is easy, drawing triangles between them dynamically is extremely difficult. Besides, if I were to go down this path, I would simply be reinventing a wheel that had already been fine tuned over decades by much smarter people. So I decided to dial down my scope. I began to think of 2D calculations that could lead to 3D representations and the solution of man ipulating landscapes only became clear. Landscapes are essentially 1D in nature - They are a collection of points with fixed x and z co-ordinates (width and length) with the only thing changing constantly being the y (height). If I were to create a tool for sculpting landscapes, it would be far easier to calculate the triangles and consequently even end up finishing my thesis.

#### So it's like clay sculpting. Well then, what is clay sculpting?
having decided I want to do 3D Sculpting for landscapes, I spent some time trying to think of what makes clay sculpting clay sculpting. I decided to buy play dough and experiment with modelling my own landscapes physically in pursuit of trying to understand the process and all it entails. While I didn't gather any tangible observations from it during this period ( I get into these observations in a later month), it set me up well towards thinking about tactile modelling.     

### July 16 - July 31
#### What's wrong with Blender and Unreal? - Where's the disconnect?
But virtual clay modelling is not a new idea! It exists, very much. Programmes like Unreal Engine, Blender, Cinema4D, ZBrush, etc. use virtual clay to model 3D characters and environments. Before I could set off inventing better clay modelling, I decided to understand what was wrong with current clay modelling. This was done through a mix of using these programmes one after the other (except the paid ones) and then switching to physical clay modelling top recognise the gaps in the experience that make them less intuitive. At the end of the process, I came up with the following observations: 
- 2D means of visualisation (monitor) and manipulation (mouse and keyboard) are detrimental to the intuitiveness.
- The softwares are filled with a variety of 'brushes' and tools which is ioverwhelming.
- A lot of these brushes save the simplest ones have complex behaviours that do not match our expectations from there
- There is no haptic, audio, (or olfactory) feedback like one would expect from physical clay.
- Virtual clay doesn't behave well when very complex deformations are done on it.
- Virtual clay needs to be remeshed in order to maintain fidelity.
- Being unable to visualise things at scale in 3D dimensions is detrimental to the cause
     
#### First trials with Kinect - This is impossible!!!
In order to solve for these gaps, I identified the need to build a new input system for these applications that relies on physical means of interactions and can provide feedback beyond mere visual one. I thought of using the Kinect 2, which I had previously attempted at harnessing in Semester 2. The idea was to let the Kinect's blob detection and limb detection determine the shape and location of my hands and to then 'insert them' into the Blender to manipulate objects. I decided to check out a Kinect from the CCI and started experimenting with it but the more I experimented the clearer it became why the Kinect isn't used as heavily as its marketing pitch would make one believe it could be:
- Not Supported: First, the Kinect 2 which is the most open easy-to-use Kinect is not supported regularly by Microsoft. Having been replaced by Azure Kinect officially and the hardware discountinued, the support for troubleshooting is limited. Most forums for the Kinect are dated and most code is made by third party enthusisasts meaning the gaps left in it that need to be addresses in order to use it are immense.
- Hard to Integrate: Input data from the Kinect can be visualised easily using the SDKs Microsoft provides (albeit with a complicated installation process personally speaking), but those values are hard to bring over into code that can be written into other thrid party software. For example, I tried using Java ion Processing and Python in Blender (with libraries people had biuilt for each) but only ended up getting 2D visualisations of the depth camera after a lot of programming. I couldn't find any way to get the Kinect to detect blobs and pass blob locations directly, meaning a lot of effort would have to go into reconstructing the existing blob detection specifically for my software.
- No Support Beyond basic Hands and legs: While Hands and legs can be easily recognised, tools that one needs to hold in their hands to sculpt are a different story.

#### Do I need to rely only on hands? - 'How Things Shape the Mind: A Theory of Material'
Another conflict I was trying to figure out was - Does it have to be just hands? Using one's hands to manipulate virtual clay would make it the most intuitive means possible sincve the cognitive load would be the least. But sculpting also sometimes necessitates the use of specific tools for the sort of precision one's fingers cannot offer. I could not decide whther I want to restrict my software to only be able to manipulate using hands or integrate sculpting tools in the software when Phoenix pointed me to an excellent book that settled this conflict. Lamfouris' "How Things Shape the Mind: A Theory of Material' speaks of how tools become essentially an extensiomn of one's hands - that using these over time makes them as though they're a part of us,  that we learn to think through tools and their affordances. It is at this point that I decided that it was okay to replicate tools as well as they wouldn't compromise on the intuitiveness I was going for.

## August
### August 01 - August 15 
#### Continued trials with Kinect - Still Impossible!!
#### Looking at more projects and literature - Categorising?
#### There's a whole taxonomy paper in here! - Literature Review
### August 16 - August 31
#### Something with this is off! - It needs to be visualised in 3D as well
#### More literature review - Finding the gap
## September
### September 01 - September 10 
#### Chat with Lieven - Let's put some VR in here, that should do it!
#### Looking at 3D Engines in detail - Unity, Unreal, OpenGL, OpenFrameworks, Processing?
#### Unity is the solution! - But everyone suddenly hates it??
### September 11 - September 20
#### Time I started developing! - Making the VR Rig in Unity
#### Small tangent into Unreal - Won't work
#### I bought the Meta Quest 2 - I hate it!
#### Thesis Editing - What is my argument really? How am I framing it?
#### Hand Presence and Physics - OpenXR vs OVR Toolkit
### September 21 - September 31
#### The Mesh Object!
#### Generating a mesh procedurally
#### Perlin Noise comes in clutch once again
#### Vertex Shading - Universal Render Pipeline!
#### Talking to sculptors - What do they really do?
## October
### October 01 - October 10 
#### Now what if I move my hands just a little bit? - Deforming the mesh little by little
#### And what if I want to move around the world myself?
#### Something's off with the scale!
#### SUCCESS! - I CAN DEFORM THE MESH!
### October 11 - October 20
#### But what are all the ways in which I can deform the mesh? - Designing brushes
#### I don't want the whole thing to move! - Defining brush shapes!
#### How do I show the area of influence?
#### Looks good, but what if someone wants to take it to Blender next - The export system
#### And what if they just want to visualise it? - PNG Exports
#### I can make a lot of brushes, but many don't serve the purpose - Legacy brushes
### October 21 - October 31
#### The brushes still don't seem natural
#### Using the hand's velocity! That's it!
#### Also, visuals are not the only important thing - Haptics and Sound Design
#### Solving performance issues - 25 fps to 65 fps
#### I need to write all of this down lest I forget myself - Documenting the code and writing the paper
## November
### November 01 - November 15
#### Let's take a step back - What am I testing towards?
#### Building the testing framework
#### Making the GUI to have it testing ready
### November 16 - November 24
#### Making a video to show it off
#### Analysing the results from the testing


